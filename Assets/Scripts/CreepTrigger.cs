using System.Collections;
using UnityEngine;

public class CreepTrigger : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AudioSource creepAudioSource;
    [SerializeField] private Camera playerCamera; // optional; will fallback to Camera.main

    [Header("Follow Settings")]
    [SerializeField] private bool followPlayer = true;
    [SerializeField] private float followDistance = 2f;
    [SerializeField] private float followHeight = 1.75f;
    [SerializeField] private float followSmoothTime = 0.06f;

    [Header("Fade / Timing")]
    [SerializeField] private float fadeInTime = 1f;
    [SerializeField] private float fadeOutTime = 1.25f;
    [SerializeField] private float startDelay = 0.5f;

    [Header("Look Detection")]
    [SerializeField, Tooltip("Dot product threshold; higher = must look more directly.")]
    private float stopLookDotThreshold = 0.85f;
    [SerializeField, Tooltip("How long the player must continuously look at the source to stop it.")]
    private float lookDurationRequired = 0.35f;
    [SerializeField, Tooltip("Max distance for raycast LOS check; set large enough to cover scene.")]
    private float maxRayDistance = 50f;
    [SerializeField] private LayerMask obstructionMask = ~0;

    [Header("Creep Motion")]
    [SerializeField] private float sideDriftAmplitude = 0.3f;
    [SerializeField] private float sideDriftSpeed = 1.5f;

    [Header("Movement Detection")]
    [SerializeField] private float minMoveSpeed = 0.05f;
    [SerializeField] private float moveCheckSmooth = 0.2f;

    [Header("Re-trigger Settings")]
    [SerializeField, Tooltip("Allow the trigger to be reused after stopping?")]
    private bool allowRetrigger = true;
    [SerializeField, Tooltip("Cooldown time before it can trigger again.")]
    private float retriggerCooldown = 3f;

    private Transform playerTransform;
    private Vector3 currentVelocity = Vector3.zero;
    private Vector3 lastPlayerPos;
    private float playerSpeedSmoothed = 0f;

    private bool isFollowing = false;
    private bool isFading = false;
    private bool canTrigger = true;
    private float lookTimer = 0f;
    private Coroutine currentFadeRoutine;

    private void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
            if (playerCamera == null)
                Debug.LogWarning("CreepTrigger: No playerCamera assigned and Camera.main is null. Look detection may fail.");
        }

        if (Player.Instance != null)
            playerTransform = Player.Instance.transform;
        else if (playerCamera != null)
            playerTransform = playerCamera.transform;

        lastPlayerPos = playerTransform.position;
    }

    private void Update()
    {
        if (!isFollowing) return;

        if (followPlayer)
            HandleFollowing();

        DetectPlayerLook();
    }

    private void HandleFollowing()
    {
        // --- PLAYER MOVEMENT DETECTION ---
        Vector3 playerDelta = playerTransform.position - lastPlayerPos;
        float currentSpeed = playerDelta.magnitude / Time.deltaTime;
        playerSpeedSmoothed = Mathf.Lerp(playerSpeedSmoothed, currentSpeed, moveCheckSmooth);
        lastPlayerPos = playerTransform.position;

        bool playerIsMoving = playerSpeedSmoothed > minMoveSpeed;

        if (playerIsMoving)
        {
            // Base follow position
            Vector3 basePos = playerTransform.position - playerTransform.forward * followDistance;
            basePos.y += followHeight;

            // Add subtle side drift
            basePos += playerTransform.right * Mathf.Sin(Time.time * sideDriftSpeed) * sideDriftAmplitude;

            // Smoothly follow
            creepAudioSource.transform.position = Vector3.SmoothDamp(
                creepAudioSource.transform.position,
                basePos,
                ref currentVelocity,
                followSmoothTime
            );
        }
    }

    private void DetectPlayerLook()
    {
        if (playerCamera == null) return;

        Vector3 toSource = creepAudioSource.transform.position - playerCamera.transform.position;
        float dist = toSource.magnitude;
        if (dist < 0.001f) return;

        Vector3 dir = toSource.normalized;
        float dot = Vector3.Dot(playerCamera.transform.forward, dir);

        if (dot >= stopLookDotThreshold)
        {
            Ray ray = new Ray(playerCamera.transform.position, dir);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Min(dist + 0.05f, maxRayDistance), obstructionMask))
            {
                bool hitIsSource = (hit.transform == creepAudioSource.transform) ||
                                   Vector3.Distance(hit.point, creepAudioSource.transform.position) < 0.2f;
                if (hitIsSource)
                {
                    lookTimer += Time.deltaTime;
                    if (lookTimer >= lookDurationRequired) StopFollowing();
                }
                else lookTimer = Mathf.Max(0f, lookTimer - Time.deltaTime * 4f);
            }
            else
            {
                lookTimer += Time.deltaTime;
                if (lookTimer >= lookDurationRequired) StopFollowing();
            }
        }
        else lookTimer = Mathf.Max(0f, lookTimer - Time.deltaTime * 3f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!canTrigger || isFollowing) return;
        if (other.CompareTag("Player")) StartFollowing();
    }

    private void StartFollowing()
    {
        if (isFading || isFollowing) return;

        Debug.Log("Creep started following.");
        isFollowing = true;
        lookTimer = 0f;
        canTrigger = false;

        if (currentFadeRoutine != null) StopCoroutine(currentFadeRoutine);
        currentFadeRoutine = StartCoroutine(FadeInAfterDelay());
    }

    private IEnumerator FadeInAfterDelay()
    {
        isFading = true;
        creepAudioSource.volume = 0f;

        if (startDelay > 0f)
            yield return new WaitForSeconds(startDelay);

        if (!isFollowing)
        {
            isFading = false;
            if (allowRetrigger) StartCoroutine(RetriggerCooldown());
            yield break;
        }

        creepAudioSource.pitch = Random.Range(0.96f, 1.04f);
        creepAudioSource.Play();

        float t = 0f;
        while (t < fadeInTime)
        {
            t += Time.deltaTime;
            creepAudioSource.volume = Mathf.Lerp(0f, 1f, t / fadeInTime);
            yield return null;
        }

        creepAudioSource.volume = 1f;
        isFading = false;
    }

    private void StopFollowing()
    {
        if (isFading || !isFollowing) return;

        Debug.Log("Creep stopped following (player looked).");
        isFollowing = false;
        lookTimer = 0f;

        if (currentFadeRoutine != null) StopCoroutine(currentFadeRoutine);
        currentFadeRoutine = StartCoroutine(FadeOutAndStop());
    }

    private IEnumerator FadeOutAndStop()
    {
        isFading = true;
        float startVol = creepAudioSource.volume;
        float t = 0f;

        while (t < fadeOutTime)
        {
            t += Time.deltaTime;
            creepAudioSource.volume = Mathf.Lerp(startVol, 0f, t / fadeOutTime);
            yield return null;
        }

        creepAudioSource.Stop();
        creepAudioSource.volume = 1f;
        isFading = false;

        if (allowRetrigger)
            StartCoroutine(RetriggerCooldown());
    }

    private IEnumerator RetriggerCooldown()
    {
        yield return new WaitForSeconds(retriggerCooldown);
        canTrigger = true;
    }

    private void OnDrawGizmosSelected()
    {
        if (playerTransform == null && Player.Instance != null)
            playerTransform = Player.Instance.transform;

        if (playerTransform != null)
        {
            Gizmos.color = Color.cyan;
            Vector3 pos = playerTransform.position - playerTransform.forward * followDistance;
            pos.y += followHeight;
            Gizmos.DrawWireSphere(pos, 0.2f);
        }
    }
}
