using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAbility : MonoBehaviour
{
    [SerializeField] protected bool _canOverlapAbility = false;
    // 발동될때 추가될 태그, 발동이 종료될때 삭제될 태그
    [SerializeField] protected List<Define.GameplayTag> _tagsToAdd;
    // 발동에 필요한 태그
    [SerializeField] protected List<Define.GameplayTag> _needTags;
    // 발동할 때 있으면 안되는 태그
    [SerializeField] protected List<Define.GameplayTag> _blockTags;
    // 발동될 때 취소할 능력
    [SerializeField] protected List<Define.GameplayAbility> _cancelAbilities;
    [SerializeField] protected float _coolTime;
    protected AbilitySystemComponent _asc;
    protected int _overlapCnt = 0;

    private bool _isActivated = false;
    private bool _isCoolTime = false;
    public bool IsActivated { get { return _isActivated; } }
    private void Start()
    {
        Init();
    }
    public void SetASC(AbilitySystemComponent asc)
    {
        _asc = asc;
    }
    public void TryActivateAbility()
    {
        if (CanActivateAbility() == false) return;
        foreach(Define.GameplayTag tag in _tagsToAdd)
        {
            _asc.AddTag(tag);
        }
        ActivateAbility();
        return;
    }
    protected virtual void Init()
    {

    }
    protected virtual void ActivateAbility()
    {
        _isActivated = true;
        _overlapCnt++;
        foreach(Define.GameplayAbility ga in _cancelAbilities)
        {
            _asc.TryCancelAbilityByTag(ga);
        }
        if(_coolTime > 0 && _canOverlapAbility == false)
        {
            _isCoolTime = true;
            StartCoroutine(CoCaculateCoolTime());
        }
    }
    protected virtual bool CanActivateAbility()
    {
        if (_isCoolTime) return false;
        foreach(Define.GameplayTag tag in _needTags)
        {
            if (_asc.IsExsistTag(tag) == false) return false;
        }
        foreach(Define.GameplayTag tag in _blockTags)
        {
            if (_asc.IsExsistTag(tag)) return false;
        }
        if (_canOverlapAbility == false)
        {
            if (_isActivated) return false;
        }
        return true;
    }
    protected virtual void EndAbility()
    {
        for(int i=0;i<_overlapCnt;i++)
        {
            RemoveTags();
        }
        _isActivated = false;
        _overlapCnt = 0;
        if (_coolTime > 0 && _canOverlapAbility)
        {
            _isCoolTime = true;
            StartCoroutine(CoCaculateCoolTime());
        }
    }
    public virtual void CancelAbility()
    {
    }
    private void RemoveTags()
    {
        foreach (Define.GameplayTag tag in _tagsToAdd)
        {
            _asc.RemoveTag(tag);
        }
    }
    IEnumerator CoCaculateCoolTime()
    {
        yield return new WaitForSeconds(_coolTime);
        _isCoolTime = false;
    }
}
