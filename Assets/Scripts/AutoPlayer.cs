using UnityEngine;

public class AutoPlayer : MonoBehaviour {

    [SerializeField] private bool _active;

    public bool Active { get => _active; set => _active = value; }
    public Chart Chart { get; private set; }

    private void Awake () {

        Chart = FindObjectOfType<Chart> ();

        if (_active) Chart.AutoPlayer = this;

    }

    public void AutoPlay () {

        if (_active) {

            foreach (JudgmentLine line in Chart.JudgmentLines) {

                foreach (Note note in line.IncompleteNotes.ToArray ()) {

                    Vector3 noteLocal = Chart.GetNoteLocal (note, note.Line);
                    if (Mathf.Abs (noteLocal.y) <= 0.1f || note.State == NoteState.Progressing) {

                        note.AssignTouch (Chart.GetTouch (note.transform.position, GetTouchPhase (note)));

                    }

                }

            }

        }

    }

    private TouchPhase GetTouchPhase (Note note) {

        if (note is TapNote) return TouchPhase.Began;
        if (note is DragNote || note is FlickNote) return TouchPhase.Moved;
        if (note is HoldNote) {

            if (note.State == NoteState.Pending) return TouchPhase.Began;
            return TouchPhase.Moved;

        }
        return TouchPhase.Began;

    }

}
