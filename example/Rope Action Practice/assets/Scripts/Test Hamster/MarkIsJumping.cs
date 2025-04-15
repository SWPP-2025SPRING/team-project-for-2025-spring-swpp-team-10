using UnityEngine;

public class MarkIsJumping : StateMachineBehaviour
{
  override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
  {
    animator.SetBool("IsJumping", false);
  }
}
