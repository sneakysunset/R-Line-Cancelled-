using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Events;

public class KeyChain : Trigger
{
    public KeyScript[] keys;
    public Vector3[] keysPos;
    int nOpen;
    LineRenderer line;
    public Color open, close;

    [Space(10)]
    [Header("Editor")]
    public KeyScript Key;
    public int numberOfKeys;
    bool isOpen;
    private Transform target;
    [SerializeField, HideInInspector] public Vector3 prevPos;

    private void Start()
    {
        line = GetComponent<LineRenderer>();
        keys = new KeyScript[keysPos.Length];
        for (int i = 0; i < keysPos.Length; i++)
        {
            keys[i] = Instantiate(Key, keysPos[i], Quaternion.identity, transform);
        }

        //Génère une ligne reliant les clés et la porte.
        line.positionCount = keys.Length + 1;
        for (int i = 0; i < keys.Length; i++)
        {
            line.SetPosition(i, keys[i].transform.position);
        }

        var obj = OnKeyActivationEvent.GetPersistentTarget(0);
        Door door = null;
        if (obj.GetType().ToString() == "Door")
        {
            door = obj as Door;
        } 
        target = door.transform;
        line.SetPosition(keys.Length, target.position);

        foreach (KeyScript key in keys) key.keyChain = this;

        line.startColor = open;
        line.endColor = open;
    }

    private void Update()
    {
        //Actualise la position des points de la ligne reliant les clés et la porte.
        line.SetPosition(keys.Length, target.position);
        if (isOpen)
        {
            line.startColor = open;
            line.endColor = open;
        }
        else
        {
            line.startColor = close;
            line.endColor = close;
        }
    }

    public void KeyTriggered()
    {
        nOpen = 0;
        foreach (KeyScript key in keys)
        {
            if (key.activated == true) nOpen++;
        }
        if (nOpen == keys.Length)
        {
            OnKeyActivationEvent?.Invoke();
            activated = true;
            isOpen = true;
        }
        else
        {
            OnKeyDesactivationEvent?.Invoke();
            activated = false;
            isOpen = false;
        }
    }

    public void MovePoint(bool pA, Vector3 pos, int i)
    {
        keysPos[i] = new Vector3(pos.x, pos.y, 0);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(KeyChain))]
public class KeyChain_Editor : Editor
{
    KeyChain keyChain;

    private void OnSceneGUI() => Draw();
    private void OnEnable() => keyChain = (KeyChain)target;

    void Draw()
    {
        Vector3[] vecs = keyChain.keysPos;
        keyChain.keysPos = new Vector3[keyChain.numberOfKeys];
        for (int i = 0; i < keyChain.keysPos.Length; i++)
        {
            if (vecs.Length > i) keyChain.keysPos[i] = vecs[i];
            else if(i > 0) keyChain.keysPos[i].x = keyChain.keysPos[i - 1].x + 1;
        }
        MoveHandlesAlongObject();

        for (int i = 0; i < keyChain.keysPos.Length - 1; i++)
        {
            Handles.color = Color.black;
            Handles.DrawLine(keyChain.keysPos[i], keyChain.keysPos[i + 1]);

            Handles.color = Color.red;
            Vector3 newPosA = Handles.FreeMoveHandle(keyChain.keysPos[i], Quaternion.identity, .5f, Vector3.zero, Handles.CylinderHandleCap);
            if (keyChain.keysPos[i] != newPosA)
            {
                Undo.RecordObject(keyChain, "MovePoint");
                keyChain.MovePoint(true, newPosA, i);
            }
        }
        Handles.color = Color.red;
        Vector3 newPos = Handles.FreeMoveHandle(keyChain.keysPos[keyChain.keysPos.Length - 1], Quaternion.identity, .5f, Vector3.zero, Handles.CylinderHandleCap);
        if (keyChain.keysPos[keyChain.keysPos.Length - 1] != newPos)
        {
            Undo.RecordObject(keyChain, "MovePoint");
            keyChain.MovePoint(true, newPos, keyChain.keysPos.Length - 1);
        }
    }


    void MoveHandlesAlongObject()
    {
        if (keyChain.prevPos != keyChain.transform.position)
        {
            Vector3 movement = keyChain.transform.position - keyChain.prevPos;
            for (int i = 0; i < keyChain.keysPos.Length; i++) keyChain.keysPos[i] += movement;
            keyChain.prevPos = keyChain.transform.position;
        }
    }
}

#endif
