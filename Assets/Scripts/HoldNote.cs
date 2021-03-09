using UnityEngine;

public class HoldNote : Note {

    [SerializeField] protected GameObject _goodTap;
    [SerializeField] protected GameObject _perfectTap;

    public override bool OnInteractionCheck (Touch touch, ref float min) {

        if (State == NoteState.Pending) {

            if (touch.phase != TouchPhase.Began) return false;

            Vector3 noteLocal = Chart.GetNoteLocal (this, Line);
            if (Mathf.Abs (noteLocal.y) <= Mathf.Min (1, min)) {

                if (Mathf.Abs (noteLocal.x - Chart.GetTouchLocal (touch, Line).x) <= 1) {

                    min = Mathf.Abs (noteLocal.y);
                    return true;

                }

            }

            return false;

        } else if (State == NoteState.Progressing) {

            if (touch.phase != TouchPhase.Stationary && touch.phase != TouchPhase.Moved) return false;

            Vector3 noteLocal = Line.transform.InverseTransformPoint (transform.position);
            if (Mathf.Abs (noteLocal.y) <= 1) {

                if (Mathf.Abs (noteLocal.x - Chart.GetTouchLocal (touch, Line).x) <= 2) {

                    AssignTouch (touch);

                    return true;

                }

            }

            return false;

        } else return false;

    }

    public override int PostInteractionCheck () {

        if (State == NoteState.Pending) {

            if (!TouchAssigned) return 0;

            _audio.Play ();

            State = NoteState.Progressing;

            float dif = Mathf.Abs (Chart.GetNoteLocal (this, Line).y - Chart.GetTouchLocal (Touch, Line).y);

            if (dif > 0.5f) {

                Instantiate (_goodTap, transform.position, Quaternion.identity, Chart.transform);
                return 25;

            }

            Instantiate (_perfectTap, transform.position, Quaternion.identity, Chart.transform);
            return 50;

        } else if (State == NoteState.Progressing) {

            if (TouchAssigned) {

                Vector3 noteLocal = Line.transform.InverseTransformPoint (transform.position);
                if (Mathf.Abs (noteLocal.y) >= Length) {

                    transform.parent = null;

                    LeanTween.value (gameObject, 1, 0, 0.1f).setOnUpdateParam (gameObject).setOnUpdateObject ((float value, object obj) => {

                        _renderer.color = new Color (1, 1, 1, value);

                    });

                    Line.IncompleteNotes.Remove (this);
                    State = NoteState.Ended;

                    Instantiate (_perfectTap, transform.position + transform.up * Length, Quaternion.identity, Chart.transform);
                    return 50;

                }

                if (noteLocal.y < 0) {

                    Vector3 targetLocal = noteLocal;
                    targetLocal.Scale (new Vector3 (1, 0, 1));
                    transform.position = Line.transform.TransformPoint (targetLocal);
                    _renderer.size = new Vector2 (Width, Length + noteLocal.y);

                }

                return 0;

            } else {

                LeanTween.value (gameObject, 1, 0.5f, 0.1f).setOnUpdateParam (gameObject).setOnUpdateObject ((float value, object obj) => {

                    _renderer.color = new Color (1, 1, 1, value);

                });

                Line.IncompleteNotes.Remove (this);
                State = NoteState.Ended;

                return 0;

            }

        } else return 0;

    }

}
