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
        
        // emote inputs
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            if (_emotePack.Emotes.Count > 0)
                PlayEmote(_emotePack.Emotes[0]);
        } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            if (_emotePack.Emotes.Count > 1)
                PlayEmote(_emotePack.Emotes[1]);
        } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            if (_emotePack.Emotes.Count > 2) 
                PlayEmote(_emotePack.Emotes[2]);
        } else if (Input.GetKeyDown(KeyCode.Alpha4)) {
            if (_emotePack.Emotes.Count > 3)
                PlayEmote(_emotePack.Emotes[3]);
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
        
        foreach (string animationName in emote.AnimationNames) {
            _animator.Play(animationName);
        }
        // get the length of the animation clip
        float clipLength = _animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        yield return new WaitForSeconds(clipLength * emote.LoopAmount);
        
        _animator.CrossFade("Base Layer.Idle_A", 0.35f);
        _animator.CrossFade("Shapekey.Eyes_Blink", 0.15f);
        // _animator.Play("Idle");
        _emoteCoroutine = null;
    }

    private bool IsEmoting() {
        return _emoteCoroutine != null;
    }
}
