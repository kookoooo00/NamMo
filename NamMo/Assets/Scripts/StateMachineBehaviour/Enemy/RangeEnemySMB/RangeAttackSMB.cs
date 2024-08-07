using Enemy;
using UnityEngine;
using UnityEngine.Animations;

namespace NamMo
{
    public class RangeAttackSMB : SceneLinkedSMB<RangedEnemy>
    {
        public override void OnSLStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            
        }

        public override void OnSLStatePostEnter(Animator animator,AnimatorStateInfo stateInfo,int layerIndex) {
            _monoBehaviour.RangeAttack();   
        }
        public override void OnSLStateNoTransitionUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex,
            AnimatorControllerPlayable controller)
        {
            
        }
        public override void OnSLStateExit (Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            _monoBehaviour.SetRangeAttack(false);
        }
    }
}