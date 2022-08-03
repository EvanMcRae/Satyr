using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UGidle : StateMachineBehaviour
{
    public UGboss undergroundBoss;
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        undergroundBoss = GameObject.FindGameObjectWithTag("UGboss").GetComponent<UGboss>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        undergroundBoss.IdleState();
    }

}
