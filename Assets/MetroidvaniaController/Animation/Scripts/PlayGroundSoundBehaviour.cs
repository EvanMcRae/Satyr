using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayGroundSoundBehaviour : StateMachineBehaviour
{
    private AudioSource[] audioSource;
    private AudioSource currSource;
    public AudioClip[] audioSounds;
    public bool loop = false;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        audioSource = animator.transform.GetComponents<AudioSource>();
        foreach (AudioSource source in audioSource)
        {
            if (!source.isPlaying)
            {
                currSource = source;
                break;
            }
        }
        currSource.clip = audioSounds[(int)Player.controller.currentGround];
        currSource.loop = loop;
        currSource.Play();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Player.controller.switchedGround)
        {
            currSource.Stop();
            currSource.clip = audioSounds[(int)Player.controller.currentGround];
            currSource.Play();
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        currSource.Stop();
        currSource.loop = false;
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
