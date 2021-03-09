using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Chart : MonoBehaviour {

    [SerializeField] private int _points;
    [SerializeField] private float _delay;
    [Range (0, 1)] public float ChromaticAberration;
    private Camera _camera;
    private Volume _volume;
    private AudioSource _audio;
    private Animation _animation;
    private float _wait;
    private bool _animStarted;
    private bool _audioStarted;

    public int Points { get => _points; set => _points = value; }
    public JudgmentLine[] JudgmentLines { get; private set; }
    public Note[] Notes { get; private set; }
    public AutoPlayer AutoPlayer { get; set; }

    private void Awake () {

        _camera = FindObjectOfType<Camera> ();
        _volume = FindObjectOfType<Volume> ();
        _audio = GetComponent<AudioSource> ();
        _animation = GetComponent<Animation> ();
        _animStarted = false;
        _audioStarted = false;

        Points = 0;
        JudgmentLines = GetComponentsInChildren<JudgmentLine> ();
        Notes = GetComponentsInChildren<Note> ();

    }

    private void Update () {

        if (!_animStarted) {

            Debug.Log ("Playing chart animation");
            _animation.Play ();
            _wait = _delay;

            _animStarted = true;

        }
        if (_wait > 0) _wait -= Time.deltaTime;
        if (_wait <= 0 && !_audioStarted) {

            Debug.Log ("Playing chart music");
            _audio.Play ();

            _audioStarted = true;

        }

        TapNotes ();
        SetVolumeParameters ();

    }

    private void TapNotes () {

        foreach (Note note in Notes) {

            note.PreInteractionCheck ();

        }

        if (AutoPlayer != null) AutoPlayer.AutoPlay ();

        // Mouse
        if (Input.GetMouseButtonDown (0)) {

            Note tapped = null;
            float min = float.MaxValue;
            foreach (JudgmentLine line in JudgmentLines) {

                foreach (Note note in line.IncompleteNotes.ToArray ()) {

                    if (note.OnInteractionCheck (GetTouch (Input.mousePosition, TouchPhase.Began, false), ref min)) {

                        tapped = note;

                    }

                }

            }
            if (tapped != null) tapped.AssignTouch (GetTouch (Input.mousePosition, TouchPhase.Began, false));

        } else if (Input.GetMouseButton (0)) {

            Note tapped = null;
            float min = float.MaxValue;
            foreach (JudgmentLine line in JudgmentLines) {

                foreach (Note note in line.IncompleteNotes.ToArray ()) {

                    if (note.OnInteractionCheck (GetTouch (Input.mousePosition, TouchPhase.Moved, false), ref min)) {

                        tapped = note;

                    }

                }

            }
            if (tapped != null) tapped.AssignTouch (GetTouch (Input.mousePosition, TouchPhase.Moved, false));

        } else if (Input.GetMouseButtonUp (0)) {

            Note tapped = null;
            float min = float.MaxValue;
            foreach (JudgmentLine line in JudgmentLines) {

                foreach (Note note in line.IncompleteNotes.ToArray ()) {

                    if (note.OnInteractionCheck (GetTouch (Input.mousePosition, TouchPhase.Ended, false), ref min)) {

                        tapped = note;

                    }

                }

            }
            if (tapped != null) tapped.AssignTouch (GetTouch (Input.mousePosition, TouchPhase.Ended, false));

        }

        // Touches
        Touch[] touches = Input.touches;
        foreach (Touch touch in touches) {

            Vector3 worldPos = _camera.ScreenToWorldPoint (touch.position);
            Note tapped = null;
            float min = float.MaxValue;
            foreach (JudgmentLine line in JudgmentLines) {

                Vector3 localPos = line.transform.InverseTransformPoint (worldPos);
                foreach (Note note in line.IncompleteNotes.ToArray ()) {

                    if (note.OnInteractionCheck (touch, ref min)) {

                        tapped = note;

                    }

                }

            }
            if (tapped != null) tapped.AssignTouch (touch);

        }

        foreach (Note note in Notes) {

            Points += note.PostInteractionCheck ();

        }

    }

    private void SetVolumeParameters () {

        VolumeProfile profile = _volume.profile;

        ChromaticAberration chromaticAberration;
        if (!profile.TryGet (out chromaticAberration)) {
            chromaticAberration = profile.Add<ChromaticAberration> (false);
        }

        chromaticAberration.intensity.Override (ChromaticAberration);

    }

    public Touch GetTouch (Vector3 pos, TouchPhase phase, bool world = true) {

        Touch touch = new Touch ();
        if (world) pos = WorldToScreen (pos);
        touch.position = pos;
        touch.phase = phase;
        return touch;

    }

    public Vector3 WorldToScreen (Vector3 world) {

        return _camera.WorldToScreenPoint (world);

    }

    public Vector3 GetNoteLocal (Note note, JudgmentLine line) {

        return line.transform.InverseTransformPoint (note.transform.position);

    }

    public Vector3 GetTouchLocal (Touch touch, JudgmentLine line) {

        Vector3 camWorld = _camera.ScreenToWorldPoint (touch.position);
        return line.transform.InverseTransformPoint (camWorld);

    }

}
