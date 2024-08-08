using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    void Interact(GameObject Interactor);
    void Focus(GameObject Interactor);
    void Unfocus(GameObject Interactor);
}
