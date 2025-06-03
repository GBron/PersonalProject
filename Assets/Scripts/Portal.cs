using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] Material _material;
    [SerializeField] ParticleSystem _particle;

    private bool _canEnter;

    private void Update()
    {
        if(GameManager.Instance.EnemyCount > 0)
        {
            _material.color = Color.red;
            _material.SetColor("_EmissionColor", Color.red);
            var ps = _particle.main;
            ps.startColor = Color.red;
            _canEnter = false;
        }
        else 
        {
            _material.color = Color.green;
            _material.SetColor("_EmissionColor", Color.green);
            var ps = _particle.main;
            ps.startColor = Color.green;
            _canEnter = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 3 && _canEnter)
        {
            GameManager.Instance.ChangeScene(2);
            GameManager.Instance.IsTimeStop = true;
        }
    }
}
