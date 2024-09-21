using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagController : MonoBehaviour
{
    [SerializeField] GameObject HitTag;
    [SerializeField] GameObject DestroyedTag;
    [SerializeField] GameObject MissedTag;
    [SerializeField] GameObject StartMissionTag;
    [SerializeField] GameObject MissionUpdatedTag;
    [SerializeField] GameObject MissionFailedTag;
    [SerializeField] GameObject MissionAccomplishedTag;

    // ��� �±׵��� ����Ʈ�� ����
    private List<GameObject> allTags = new List<GameObject>();

    private void Start()
    {
        // �±׵��� ����Ʈ�� �߰�
        allTags.Add(HitTag);
        allTags.Add(DestroyedTag);
        allTags.Add(MissedTag);
        allTags.Add(StartMissionTag);
        allTags.Add(MissionUpdatedTag);
        allTags.Add(MissionFailedTag);
        allTags.Add(MissionAccomplishedTag);
    }

    // Ư�� �±׸� ȣ���Ͽ� Ȱ��ȭ�ϰ� 2�� �� ��Ȱ��ȭ
    public void ActivateTag(GameObject tag)
    {
        // �ٸ� �±׵��� ��Ȱ��ȭ
        DeactivateAllTags();

        // ������ �±� Ȱ��ȭ
        tag.SetActive(true);

        // 2�� �Ŀ� �ش� �±� ��Ȱ��ȭ
        StartCoroutine(DeactivateAfterDelay(tag, 2f));
    }

    // 2�� �ڿ� �±� ��Ȱ��ȭ�ϴ� �ڷ�ƾ
    private IEnumerator DeactivateAfterDelay(GameObject tag, float delay)
    {
        yield return new WaitForSeconds(delay);
        tag.SetActive(false);
    }

    // ��� �±� ��Ȱ��ȭ
    private void DeactivateAllTags()
    {
        foreach (GameObject tag in allTags)
        {
            tag.SetActive(false);
        }
    }

    // ���÷� �±׵��� ���� ȣ���ϴ� �Լ�
    public void ShowHitTag()
    {
        ActivateTag(HitTag);
    }

    public void ShowDestroyedTag()
    {
        ActivateTag(DestroyedTag);
    }

    public void ShowMissedTag()
    {
        ActivateTag(MissedTag);
    }

    public void ShowStartMissionTag()
    {
        ActivateTag(StartMissionTag);
    }

    public void ShowMissionUpdatedTag()
    {
        ActivateTag(MissionUpdatedTag);
    }

    public void ShowMissionFailedTag()
    {
        ActivateTag(MissionFailedTag);
    }

    public void ShowMissionAccomplishedTag()
    {
        ActivateTag(MissionAccomplishedTag);
    }
}
