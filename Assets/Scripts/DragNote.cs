using UnityEngine;

public class DragNote : Note {

    [SerializeField] protected GameObject _perfectTap;

    public override bool OnInteractionCheck (Touch touch, ref float min) {

        if (State != NoteState.Pending) return false;

        if (touch.phase != TouchPhase.Stationary && touch.phase != TouchPhase.Moved) return false;

        Vector3 noteLocal = Chart.GetNoteLocal (this, Line);
        if (Mathf.Abs (noteLocal.y) <= Mathf.Min (0.5f, min)) {

            if (Mathf.Abs (noteLocal.x - Chart.GetTouchLocal (touch, Line).x) <= 2) {

                min = Mathf.Abs (noteLocal.y);
                return true;

            }

        }

        return false;

    }

    public override int PostInteractionCheck () {

        if (State == NoteState.Pending) {

            if (!TouchAssigned) return 0;

            Line.IncompleteNotes.Remove (this);
            State = NoteState.Progressing;

            return 100;

        } else if (State == NoteState.Progressing) {

            if (Chart.GetNoteLocal (this, Line).y <= 0) {

                transform.parent = null;

                LeanTween.value (gameObject, 1, 0, 0.1f).setOnUpdateParam (gameObject).setOnUpdateObject ((float value, object obj) => {

                    _renderer.color = new Color (1, 1, 1, value);

                });

                _audio.Play ();

                State = NoteState.Ended;

                Instantiate (_perfectTap, transform.position, Quaternion.identity, Chart.transform);

            }

            return 0;

        } else return 0;

    }

}
