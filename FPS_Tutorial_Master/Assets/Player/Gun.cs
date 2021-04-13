using UnityEngine;

public class Gun : MonoBehaviour {

    Transform cam;

    [SerializeField] float range = 50f;
    [SerializeField] float damage = 10f;

    private void Awake ()
    {
        cam = Camera.main.transform;
    }

    public void Shoot ()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, range)) {
            if (hit.collider.GetComponent<Damageable>() != null) {
                hit.collider.GetComponent<Damageable>().TakeDamage(damage, hit.point, hit.normal);
            }
        }
    }

}