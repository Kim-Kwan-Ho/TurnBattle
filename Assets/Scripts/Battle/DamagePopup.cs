using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{
    private TextMeshPro _text = null;
    private float _lifeTime = 2;
    private void Awake()
    {
        _text = gameObject.GetComponent<TextMeshPro>();
    }

    private void Update()
    {
        _lifeTime -= Time.deltaTime;
        if (_lifeTime < 0)
            Destroy(this.gameObject);
    }
    public void SetText(string damage)
    {
        _text.text = damage;
    }
}
