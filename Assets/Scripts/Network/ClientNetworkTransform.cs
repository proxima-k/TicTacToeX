using System.Collections;
using System.Collections.Generic;
using Unity.Netcode.Components;
using UnityEngine;

public class ClientNetworkTransform : NetworkTransform
{
    // this makes it so that the client has authority over the object
    protected override bool OnIsServerAuthoritative() {
        return false;
    }
}
