using UnityEngine;
using System.Collections;
using System.Security.Cryptography;
using UnityEditor;

public class NoteController : MonoBehaviour
{
    public NoteData NoteData;

    [SerializeField]
    MeshRenderer noteColorRenderer;

    [SerializeField]
    Material redMaterial;
    [SerializeField]
    Material blueMaterial;

    [SerializeField]
    int redLayer;
    [SerializeField]
    int blueLayer;

    [SerializeField]
    Transform visualTransform;

    void Update()
    {
        transform.position -= new Vector3(0, 0, 18) * Time.deltaTime;

        if (transform.position.z <= -2)
        {
            Destroy(gameObject);
        }

    }

    public void Setup(NoteData noteData)
    {
        NoteData = noteData;

        SetType(noteData.Type);
        SetCutDirection(noteData.CutDirection);
    }

    void SetType(int type)
    {
        if (type == 0)
        {
            noteColorRenderer.material = redMaterial;
            transform.parent.gameObject.layer = redLayer - 1;
        }
        else
        {
            noteColorRenderer.material = blueMaterial;
            transform.parent.gameObject.layer = blueLayer - 1;
        }
    }

    void SetCutDirection(int cutDirection)
    {
        switch ((CutDirection)cutDirection)
        {
            case CutDirection.Upwards:
                transform.Rotate(new Vector3(0, 0, 180), Space.Self);
                break;
            case CutDirection.Downwards:
                break;
            case CutDirection.TowardsLeft:
                transform.Rotate(new Vector3(0, 0, -90), Space.Self);
                break;
            case CutDirection.TowardsRight:
                transform.Rotate(new Vector3(0, 0, 90), Space.Self);
                break;
            case CutDirection.TowardsTopLeft:
                transform.Rotate(new Vector3(0, 0, -135), Space.Self);
                break;
            case CutDirection.TowardsTopRight:
                transform.Rotate(new Vector3(0, 0, 135), Space.Self);
                break;
            case CutDirection.TowardsBottomLeft:
                transform.Rotate(new Vector3(0, 0, -45), Space.Self);
                break;
            case CutDirection.TowardsBottomRight:
                transform.Rotate(new Vector3(0, 0, 45), Space.Self);
                break;
            case CutDirection.Any:
                visualTransform.localScale = new Vector3(0.05f, 0.1f, 0.1f);
                visualTransform.position = new Vector3(0, 0, visualTransform.position.z);
                break;
            default:
                break;
        }
    }
}
