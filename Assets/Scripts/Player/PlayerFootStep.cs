using UnityEngine;

[System.Serializable]
public class PlayerFootStep
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float stepDistance = 2f; // distance between footsteps
    private float distanceTraveled = 0f;
    private Vector3 lastPosition;

    [SerializeField] private LayerMask woodLayer;
    [SerializeField] private LayerMask concreteLayer;
    [SerializeField] private LayerMask metalLayer;
    
    [SerializeField] private string[] tileClips;
    [SerializeField] private string[] woodClips;
    [SerializeField] private string[] concreteClips;
    [SerializeField] private string[] metalClips;

    [SerializeField] private bool debug = false;

    public void Initialize()
    {
        lastPosition = playerTransform.position;
    }

    public void HandleFootsteps()
    {
        Vector3 horizontalMovement = playerTransform.position - lastPosition;
        horizontalMovement.y = 0f;
        distanceTraveled += horizontalMovement.magnitude;

        if (distanceTraveled >= stepDistance)
        {
            PlayFootstep();
            distanceTraveled = 0f;
        }

        lastPosition = playerTransform.position;
    }

    void PlayFootstep()
    {
        RaycastHit hit;
        string[] clipsToUse = tileClips; // default

        if (Physics.Raycast(playerTransform.position, Vector3.down, out hit, 2f))
        {
            if (LayerMask.LayerToName(hit.collider.gameObject.layer) == "Metal") clipsToUse = metalClips;
            else if (LayerMask.LayerToName(hit.collider.gameObject.layer) == "Wood") clipsToUse = woodClips;
            else if (LayerMask.LayerToName(hit.collider.gameObject.layer) == "Concrete") clipsToUse = concreteClips;
            else clipsToUse = tileClips;

            if (debug)
                Debug.Log("Footstep Surface: " + hit.collider.name +" of Layer: "+ LayerMask.LayerToName(hit.collider.gameObject.layer));
        }

        if (clipsToUse.Length > 0)
        {
            string clipName = clipsToUse[Random.Range(0, clipsToUse.Length)];
            AudioManager.Instance.PlaySound(clipName);
        }
    }
}