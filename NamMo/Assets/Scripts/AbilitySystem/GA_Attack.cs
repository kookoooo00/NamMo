using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Enemy;

[Serializable]
struct AttackInfo
{
    public float attackTime;
    public float attackMoment;
    public float attackRate;
    public float dashForce;
    public float dashTime;
    public Vector2 attackRange;
    public Vector2 attackOffset;
}
public class GA_Attack : GameAbility
{
    [SerializeField] private List<AttackInfo> _comboAttackInfos = new List<AttackInfo>();
    private bool _reserveNextAttack = false;
    private Coroutine _attackCoroutine = null;

    public Action<int> OnAttackComboChanged;
    protected override void ActivateAbility()
    {
        if(_overlapCnt == 0 || _reserveNextAttack)
        {
            base.ActivateAbility();
            _attackCoroutine = StartCoroutine(CoAttack());
            _asc.GetPlayerController().GetAttackArea().OnAttackAreaTriggerEntered += HandleTriggeredObject;
        }
        else
        {
            _reserveNextAttack = true;
        }
    }
    protected override bool CanActivateAbility()
    {
        if (base.CanActivateAbility() == false) return false;
        if (_overlapCnt >= _comboAttackInfos.Count) return false;
        if (_reserveNextAttack) return false;
        return true;
    }
    public override void CancelAbility()
    {
        if (_reserveNextAttack)
        {
            _overlapCnt++;
            _reserveNextAttack = false;
        }
        _asc.gameObject.GetComponent<PlayerMovement>().CancelReserveDash(Define.DashType.AttackDash);
        EndAbility();
    }
    protected override void EndAbility()
    {
        _asc.GetPlayerController().GetAttackArea().OnAttackAreaTriggerEntered -= HandleTriggeredObject;
        if (_reserveNextAttack)
        {
            ActivateAbility();
        }
        else
        {
            base.EndAbility();
            if (OnAttackComboChanged != null) OnAttackComboChanged.Invoke(-1);
            if (_attackCoroutine != null)
            {
                StopCoroutine(_attackCoroutine);
                _attackCoroutine = null;
            }
        }
    }
    private void HandleTriggeredObject(GameObject go)
    {
        Debug.Log("Attack Hit");
        go.GetComponent<Enemy.Enemy>().Hit(1);
    }
    IEnumerator CoAttack()
    {
        _reserveNextAttack = false;
        
        int currCombo = _overlapCnt - 1;
        if (OnAttackComboChanged != null) OnAttackComboChanged.Invoke(currCombo);

        AttackInfo currComboAttackInfo = _comboAttackInfos[currCombo];
        float additionalGravityTime = currComboAttackInfo.attackTime - currComboAttackInfo.attackMoment - currComboAttackInfo.dashTime;
        _asc.gameObject.GetComponent<PlayerMovement>().ReserveDash(currComboAttackInfo.attackMoment, currComboAttackInfo.dashForce, currComboAttackInfo.dashTime, Define.DashType.AttackDash, additionalGravityTime);      
        
        yield return new WaitForSeconds(currComboAttackInfo.attackMoment);

        _asc.GetPlayerController().GetAttackArea().SetAttackRange(currComboAttackInfo.attackRange, currComboAttackInfo.attackOffset);
        _asc.GetPlayerController().GetAttackArea().ActiveAttackArea();
        
        yield return new WaitForFixedUpdate();

        _asc.GetPlayerController().GetAttackArea().DeActiveAttackArea();
        
        yield return new WaitForSeconds(currComboAttackInfo.attackTime - currComboAttackInfo.attackMoment);

        _attackCoroutine = null;
        EndAbility();
    }
}
