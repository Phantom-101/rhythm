using UnityEngine;

public class Note : MonoBehaviour {

    //[SerializeField] private int BadPoints;
    //[SerializeField] private int GoodPoints;
    //[SerializeField] private int PerfectPoints;

    [SerializeField] protected NoteState _state;
    protected SpriteRenderer _renderer;
    protected AudioSource _audio;

    public NoteState State { get => _state; protected set => _state = value; }
    public JudgmentLine Line { get; protected set; }
    public Chart Chart { get; protected set; }
    public float Angle { get => transform.eulerAngles.z; }
    public float Width { get => _renderer.size.x; }
    public float Length { get => _renderer.size.y; }
    public Touch Touch { get; set; }
    public bool TouchAssigned { get; set; }

    private void Awake () {

        _renderer = GetComponent<SpriteRenderer> ();
        _audio = GetComponent<AudioSource> ();
        State = NoteState.Pending;
        Line = GetComponentInParent<JudgmentLine> ();
        Chart = GetComponentInParent<Chart> ();

    }

    public void AssignTouch (Touch touch) {

        Touch = touch;
        TouchAssigned = true;

    }

    public virtual void PreInteractionCheck () {

        TouchAssigned = false;

    }

    public virtual bool OnInteractionCheck (Touch touch, ref float min) {

        return false;

    }

    public virtual int PostInteractionCheck () {

        return 0;

    }

}
