
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OuverturePorte : MonoBehaviour
{
    public float angleDouverture = 90f;
    public float vitesseDouverture = 1f;
    public bool estOuverte = false;

    private Quaternion _rotationFermee;
    private Quaternion _rotationOuvert;
    private Coroutine _coroutineActuelle;

    // Start is called before the first frame update
    void Start()
    {
        _rotationFermee = transform.rotation;
        _rotationOuvert = Quaternion.Euler(transform.eulerAngles + new Vector3(0, angleDouverture, 0));
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (_coroutineActuelle != null) StopCoroutine(_coroutineActuelle);
            _coroutineActuelle = StartCoroutine(ActiverPorte());
        }
    }

    private IEnumerator ActiverPorte()
    {
        Quaternion targetRotation = estOuverte ? _rotationFermee : _rotationOuvert;
        estOuverte = !estOuverte;

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.01f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * vitesseDouverture);
            yield return null;
        }

        transform.rotation = targetRotation;
    }
}