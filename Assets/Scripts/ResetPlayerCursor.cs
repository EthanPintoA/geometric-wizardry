using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetPlayerCursor : MonoBehaviour
{
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
    }
}
