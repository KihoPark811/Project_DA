// GridManager�� ���ǵ� ��(�׸���)�� ũ�⿡ ���� ī�޶��� ��ġ�� �þ߸� �ڵ����� �����մϴ�.
// � ȭ�� ����(�ػ�)������ �� ��ü�� ������ �����Ͽ� ȭ�鿡 �� ������ ������ݴϴ�.

using UnityEngine; // ����Ƽ ������ �⺻ ����� ����ϱ� ���� �����մϴ�.

// - �� ��ũ��Ʈ�� ������ ���� ������Ʈ���� �ݵ�� Camera ������Ʈ�� �ֵ��� �����մϴ�.
[RequireComponent(typeof(Camera))]
// - ī�޶� �ڵ� ���� ����� �����ϴ� Ŭ�����Դϴ�.
public class AutoCameraFit : MonoBehaviour
{
    // - �ν����� â���� GridManager ������Ʈ�� ���� ������Ʈ�� ������ �����Դϴ�.
    public GridManager grid;
    // - �� �ֺ��� �߰��� ������ ũ�⸦ �����մϴ�.
    public float margin = 0.5f;

    // - ������ ���۵� �� �� ���� ȣ��Ǵ� ����Ƽ ���� �Լ��Դϴ�.
    void Start()
    {
        // - �� ��ũ��Ʈ�� ������ ������Ʈ�� Camera ������Ʈ�� �����ɴϴ�.
        var cam = GetComponent<Camera>();
        // - ī�޶��� ���� ����� 2D�� ������ ����(Orthographic) ������� �����մϴ�.
        cam.orthographic = true;

        // --- ī�޶� ��ġ�� ���� ���߾����� �̵� ---
        // - �׸����� ���� �߾� ��ǥ�� ����մϴ�. (��ǥ�� 0���� �����ϹǷ� -1)
        float cx = (grid.width - 1) * 0.5f;
        // - �׸����� ���� �߾� ��ǥ�� ����մϴ�.
        float cy = (grid.height - 1) * 0.5f;
        // - ���� �߾� ��ǥ�� ī�޶��� ��ġ�� �����մϴ�. (z���� �ڷ� �������� ����)
        transform.position = new Vector3(cx, cy, -10f);

        // --- ȭ�� ������ ���� ī�޶� �þ�(ũ��) ���� ---
        // - ī�޶� ������� �� ��ǥ �ʺ� ����մϴ�. (�׸��� �ʺ� + ���� ����)
        float targetWidth = grid.width + margin * 2f;
        // - ī�޶� ������� �� ��ǥ ���̸� ����մϴ�. (�׸��� ���� + ���Ʒ� ����)
        float targetHeight = grid.height + margin * 2f;

        // - ���� ���� ȭ���� ����/���� ������ ����մϴ�.
        float screenAspect = (float)Screen.width / Screen.height;
        // - ī�޶� ������� �� �� ������ ����/���� ������ ����մϴ�.
        float mapAspect = targetWidth / targetHeight;

        // - ȭ�� ������ �� �������� ũ�ų� ������(��, ȭ���� �ʺ��� ������ �� ������) Ȯ���մϴ�.
        if (screenAspect >= mapAspect)
        {
            // - ȭ���� �� �����Ƿ�, ī�޶� ũ�⸦ ���� '����'�� ��Ȯ�� ����ϴ�.
            cam.orthographicSize = targetHeight * 0.5f;
        }
        // - ȭ���� �ʺ��� ���η� �� �� ����Դϴ�.
        else
        {
            // - ȭ���� �� ��Ƿ�, ī�޶� ũ�⸦ ���� '�ʺ�'�� ���ߵ�, ȭ�� ������ ����Ͽ� ����մϴ�.
            cam.orthographicSize = (targetWidth * 0.5f) / screenAspect;
        }
    }
}