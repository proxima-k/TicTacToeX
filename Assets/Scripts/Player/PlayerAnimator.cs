using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAnimator : NetworkBehaviour
{
    [SerializeField] private PlayerNetwork _playerNetwork;
    [SerializeField] private EmotePackSO _emotePack;
    
    private Animator _animator;
    private const string IS_WALKING = "IsWalking";
    private const string IS_EMOTING = "IsEmoting";
    
    private Coroutine _emoteCoroutine;

    private void Awake() {
        _animator = GetComponent<Animator>();
    }

    private void Update() {
        if (!IsOwner)
            return;

        
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            PlayEmote(_emotePack.Emotes[0]);
        }
        
        if (IsEmoting())
            return;
        
        _animator.SetBool(IS_EMOTING, false);
        _animator.SetBool(IS_WALKING, _playerNetwork.IsWalking());
    }
    
    public void PlayEmote(Emote emote) {
    
        if (emote == null) {
            Debug.LogError($"Emote {emote} not found in EmotePackSO");
            return;
        }

        if (IsEmoting())
            StopCoroutine(_emoteCoroutine);
        
        _emoteCoroutine = StartCoroutine(PlayEmoteCoroutine(emote));
    }
    
    private IEnumerator PlayEmoteCoroutine(Emote emote) {
        _animator.SetBool(IS_EMOTING, true);
        
        _animator.Play(emote.AnimationName);
        // get the length of the animation clip
        float clipLength = _animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        yield return new WaitForSeconds(clipLength * emote.LoopAmount);
        
        _animator.CrossFade("Base Layer.Idle_A", 0.15f);
        // _animator.Play("Idle");
        _emoteCoroutine = null;
    }

    private bool IsEmoting() {
        return _emoteCoroutine != null;
    }
}
