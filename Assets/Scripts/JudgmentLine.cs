using System.Collections.Generic;
using UnityEngine;

public class JudgmentLine : MonoBehaviour {

    public Note[] Notes { get; private set; }
    public List<Note> IncompleteNotes { get; private set; }
    public float Angle { get => transform.eulerAngles.z; }

    private void Awake () {

        Notes = GetComponentsInChildren<Note> ();
        IncompleteNotes = new List<Note> ();
        IncompleteNotes.AddRange (Notes);

    }

}
