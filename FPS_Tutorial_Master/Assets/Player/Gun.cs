using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {

    Transform cam;

    [Header("General Stats")]
    [SerializeField] float range = 50f;
    [SerializeField] float damage = 10f;

    [SerializeField] int maxAmmo;
    int currentAmmo;

    [SerializeField] float fireRate = 5f;
    [SerializeField] float reloadTime;
    WaitForSeconds reloadWait;

    [SerializeField] float inaccuracyDistance = 5f;

    [Header("Rapid Fire")]
    [SerializeField] bool rapidFire = false;
    WaitForSeconds rapidFireWait;

    [Header("Shotgun")]
    [SerializeField] bool shotgun = false;
    [SerializeField] int bulletsPerShot = 6;

    [Header("Laser")]
    [SerializeField] GameObject laser;
    [SerializeField] Transform muzzle;
    [SerializeField] float fadeDuration = 0.3f;

    private void Awake ()
    {
        cam = Camera.main.transform;
        rapidFireWait = new WaitForSeconds(1 / fireRate);
        reloadWait = new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
    }

    public void Shoot ()
    {
        currentAmmo--; // currentAmmo = currentAmmo - 1

        if (shotgun) {
            for (int i = 0; i < bulletsPerShot; i++) {
                RaycastHit hit;
                Vector3 shootingDir = GetShootingDirection();
                if (Physics.Raycast(cam.position, shootingDir, out hit, range)) {
                    if (hit.collider.GetComponent<Damageable>() != null) {
                        hit.collider.GetComponent<Damageable>().TakeDamage(damage, hit.point, hit.normal);
                    }
                    CreateLaser(hit.point);
                } else {
                    CreateLaser(cam.position + shootingDir * range);
                }
            }
        } else {
            RaycastHit hit;
            Vector3 shootingDir = GetShootingDirection();
            if (Physics.Raycast(cam.position, shootingDir, out hit, range)) {
                if (hit.collider.GetComponent<Damageable>() != null) {
                    hit.collider.GetComponent<Damageable>().TakeDamage(damage, hit.point, hit.normal);
                }
                CreateLaser(hit.point);
            } else {
                CreateLaser(cam.position + shootingDir * range);
            }
        }
    }

    public IEnumerator RapidFire()
    {
        if (CanShoot()) {
            Shoot();
            if (rapidFire) {
                while (CanShoot()) {
                    yield return rapidFireWait;
                    Shoot();
                }
                StartCoroutine(Reload());
            }
        } else {
            StartCoroutine(Reload());
        }
    }

    IEnumerator Reload()
    {
        if (currentAmmo == maxAmmo) {
            yield return null;
        }

        print("reloading...");
        yield return reloadWait;
        currentAmmo = maxAmmo;
        print("finished reloading");
    }

    bool CanShoot ()
    {
        bool enoughAmmo = currentAmmo > 0;
        return enoughAmmo;
    }

    Vector3 GetShootingDirection()
    {
        Vector3 targetPos = cam.position + cam.forward * range;
        targetPos = new Vector3(
            targetPos.x + Random.Range(-inaccuracyDistance, inaccuracyDistance),
            targetPos.y + Random.Range(-inaccuracyDistance, inaccuracyDistance),
            targetPos.z + Random.Range(-inaccuracyDistance, inaccuracyDistance)
            );

        Vector3 direction = targetPos - cam.position;
        return direction.normalized;
    }

    void CreateLaser(Vector3 end)
    {
        LineRenderer lr = Instantiate(laser).GetComponent<LineRenderer>();
        lr.SetPositions(new Vector3[2] { muzzle.position, end });
        StartCoroutine(FadeLaser(lr));
    }

    IEnumerator FadeLaser(LineRenderer lr)
    {
        float alpha = 1;
        while (alpha > 0) {
            alpha -= Time.deltaTime / fadeDuration;
            lr.startColor = new Color(lr.startColor.r, lr.startColor.g, lr.startColor.b, alpha);
            lr.endColor = new Color(lr.endColor.r, lr.endColor.g, lr.endColor.b, alpha);
            yield return null;
        }
    }

}