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

    // 모든 태그들을 리스트로 관리
    private List<GameObject> allTags = new List<GameObject>();

    private void Start()
    {
        // 태그들을 리스트에 추가
        allTags.Add(HitTag);
        allTags.Add(DestroyedTag);
        allTags.Add(MissedTag);
        allTags.Add(StartMissionTag);
        allTags.Add(MissionUpdatedTag);
        allTags.Add(MissionFailedTag);
        allTags.Add(MissionAccomplishedTag);
    }

    // 특정 태그를 호출하여 활성화하고 2초 뒤 비활성화
    public void ActivateTag(GameObject tag)
    {
        // 다른 태그들을 비활성화
        DeactivateAllTags();

        // 지정된 태그 활성화
        tag.SetActive(true);

        // 2초 후에 해당 태그 비활성화
        StartCoroutine(DeactivateAfterDelay(tag, 2f));
    }

    // 2초 뒤에 태그 비활성화하는 코루틴
    private IEnumerator DeactivateAfterDelay(GameObject tag, float delay)
    {
        yield return new WaitForSeconds(delay);
        tag.SetActive(false);
    }

    // 모든 태그 비활성화
    private void DeactivateAllTags()
    {
        foreach (GameObject tag in allTags)
        {
            tag.SetActive(false);
        }
    }

    // 예시로 태그들을 각각 호출하는 함수
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
