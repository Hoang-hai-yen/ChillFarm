using System.Collections.Generic;
using UnityEngine;

public class DialogData : MonoBehaviour
{
    public Dialog CreateDialog()
    {
        return new Dialog(
            "Chào bạn! Dạo này bạn có khỏe không?",
            new List<DialogChoice>
            {
                new DialogChoice(
                    "Tôi khỏe, cảm ơn bạn. Còn bạn?",
                    "Tôi cũng khỏe, cảm ơn!",
                    () => DialogManager.Instance.StartCoroutine(DialogManager.Instance.ShowDialog(CreateFollowUpDialogAfterHealthy()))
                ),
                new DialogChoice(
                    "Tôi hơi mệt, đang bị cảm.",
                    "Ôi, vậy bạn nên nghỉ ngơi.",
                    () => DialogManager.Instance.StartCoroutine(DialogManager.Instance.ShowDialog(CreateFollowUpDialogAfterSick()))
                )
            }
        );
    }

    public Dialog CreateFollowUpDialogAfterHealthy()
    {
        Dialog[] options = new Dialog[]
        {
            new Dialog(
                "Mình cũng khỏe, cảm ơn bạn! Hôm nay bạn có định đi chợ không?",
                new List<DialogChoice>
                {
                    new DialogChoice(
                        "Mình sẽ đi chợ cùng bạn.",
                        "Tuyệt quá! Chúng ta cùng đi nhé.",
                        () => DialogManager.Instance.StartCoroutine(DialogManager.Instance.ShowDialog(CreateGreatFollowUpHealthy()))
                    ),
                    new DialogChoice(
                        "Hôm nay mình bận, để hôm khác.",
                        "Không sao, hôm khác chúng ta cùng đi chợ nhé!",
                        () => DialogManager.Instance.StartCoroutine(DialogManager.Instance.ShowDialog(CreateOkFollowUpHealthy()))
                    )
                }
            ),
            new Dialog(
                "Hôm nay trời đẹp quá! Bạn có muốn cùng mình dạo quanh chợ không?",
                new List<DialogChoice>
                {
                    new DialogChoice(
                        "Rất muốn, mình đi cùng bạn.",
                        "Tuyệt vời! Mình sẽ dẫn bạn qua các gian hàng thú vị.",
                        () => DialogManager.Instance.StartCoroutine(DialogManager.Instance.ShowDialog(CreateGreatFollowUpHealthy()))
                    ),
                    new DialogChoice(
                        "Hôm nay mình bận, lần khác nhé.",
                        "Không sao, chúng ta sẽ đi vào dịp khác!",
                        () => DialogManager.Instance.StartCoroutine(DialogManager.Instance.ShowDialog(CreateOkFollowUpHealthy()))
                    )
                }
            ),
            new Dialog(
                "Bạn có muốn thử món ăn đặc sản ở chợ hôm nay không?",
                new List<DialogChoice>
                {
                    new DialogChoice(
                        "Có chứ, mình rất thích.",
                        "Tuyệt quá! Mình sẽ dẫn bạn đến chỗ ngon nhất.",
                        () => DialogManager.Instance.StartCoroutine(DialogManager.Instance.ShowDialog(CreateGreatFollowUpHealthy()))
                    ),
                    new DialogChoice(
                        "Hôm nay mình không ăn được, cảm ơn bạn.",
                        "Không sao, lần khác mình sẽ dẫn bạn thử nhé!",
                        () => DialogManager.Instance.StartCoroutine(DialogManager.Instance.ShowDialog(CreateOkFollowUpHealthy()))
                    )
                }
            )
        };

        int randomIndex = Random.Range(0, options.Length);
        return options[randomIndex];
    }

    public Dialog CreateFollowUpDialogAfterSick()
    {
        return new Dialog(
            "Bạn nên nghỉ ngơi và uống thuốc đầy đủ. Bạn đã uống thuốc chưa?",
            new List<DialogChoice>
            {
                new DialogChoice(
                    "Chưa, mình sẽ uống ngay.",
                    "Tốt lắm! Nghỉ ngơi đủ sẽ giúp bạn nhanh khỏe lại.",
                    () => DialogManager.Instance.StartCoroutine(DialogManager.Instance.ShowDialog(CreateGoodFollowUpSick()))
                ),
                new DialogChoice(
                    "Vâng, bạn giúp mình mua thuốc nhé.",
                    "Được, mình sẽ chuẩn bị thuốc và hướng dẫn bạn cách sử dụng đúng liều.",
                    () => DialogManager.Instance.StartCoroutine(DialogManager.Instance.ShowDialog(CreateHelpFollowUpSick()))
                )
            }
        );
    }

    public Dialog CreateGreatFollowUpHealthy()
    {
        return new Dialog(
            "Tuyệt quá! Chúng ta cùng đi chợ và mua những thứ cần thiết nhé!",
            null
        );
    }

    public Dialog CreateOkFollowUpHealthy()
    {
        return new Dialog(
            "Không sao, hôm khác chúng ta cùng đi chợ nhé!",
            null
        );
    }

    public Dialog CreateGoodFollowUpSick()
    {
        return new Dialog(
            "Tốt lắm! Nghỉ ngơi đủ sẽ giúp bạn nhanh khỏe lại.",
            null
        );
    }

    public Dialog CreateHelpFollowUpSick()
    {
        return new Dialog(
            "Được, mình sẽ chuẩn bị thuốc và hướng dẫn bạn cách sử dụng đúng liều.",
            null
        );
    }
}
