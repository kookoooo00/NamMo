using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : MonoBehaviour
{
    [SerializeField] private float _maxHp;
    [SerializeField] private float _hp;

    private PlayerController _pc;

    public Action OnDead;
    public Action<float, float> OnHpChanged;
    public float MaxHp { get { return _maxHp; } }
    public float Hp { get { return _hp; } }
    private void Start()
    {
        OnHpChanged += TestChangeHp;
    }
    public void SetPlayerController(PlayerController pc)
    {
        _pc = pc;
    }
    public void SetHealthInfo(float hp, float maxHp)
    {
        _hp = hp;
        _maxHp = maxHp;
        if (OnHpChanged != null) OnHpChanged.Invoke(_hp, _maxHp);
    }
    public void ApplyDamage(float damage)
    {
        if (_hp <= damage)
        {
            _hp = 0;
            if (OnDead != null) OnDead.Invoke();
        }
        else
        {
            _hp -= damage;
        }
        if (OnHpChanged != null) OnHpChanged.Invoke(_hp, _maxHp);
    }
    public void ApplyHeal(float heal)
    {
        _hp += heal;
        if (_hp > _maxHp) _hp = _maxHp;
        if (OnHpChanged != null) OnHpChanged.Invoke(_hp, _maxHp);
    }
    private void TestChangeHp(float hp, float maxHp)
    {
        //Debug.Log($"Hp : {hp}, MaxHp : {maxHp}");
    }
}
