using UnityEngine;

public class TapNote : Note {

    [SerializeField] protected GameObject _goodTap;
    [SerializeField] protected GameObject _perfectTap;

    public override bool OnInteractionCheck (Touch touch, ref float min) {

        if (State != NoteState.Pending) return false;

        if (touch.phase != TouchPhase.Began) return false;

        Vector3 noteLocal = Chart.GetNoteLocal (this, Line);
        if (Mathf.Abs (noteLocal.y) <= Mathf.Min (2, min)) {

            if (Mathf.Abs (noteLocal.x - Chart.GetTouchLocal (touch, Line).x) <= 1) {

                min = Mathf.Abs (noteLocal.y);
                return true;

            }

        }

        return false;

    }

    public override int PostInteractionCheck () {

        if (!TouchAssigned) return 0;

        if (State != NoteState.Pending) return 0;

        transform.parent = null;

        LeanTween.value (gameObject, 1, 0, 0.1f).setOnUpdateParam (gameObject).setOnUpdateObject ((float value, object obj) => {

            _renderer.color = new Color (1, 1, 1, value);

        });

        _audio.Play ();

        Line.IncompleteNotes.Remove (this);
        State = NoteState.Ended;

        float dif = Mathf.Abs (Chart.GetNoteLocal (this, Line).y - Chart.GetTouchLocal (Touch, Line).y);

        if (dif > 1) return 0;

        if (dif > 0.5f) {

            Instantiate (_goodTap, transform.position, Quaternion.identity, Chart.transform);
            return 75;

        }

        Instantiate (_perfectTap, transform.position, Quaternion.identity, Chart.transform);
        return 100;

    }

}
