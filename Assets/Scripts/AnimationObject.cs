using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//キャラクターのアニメーションデータを書き出して実行するオブジェクト
public class AnimationObject : MonoBehaviour
{
    [System.Serializable]
    public struct TextureSendData
    {
        public Renderer meshRenderer;
        public int index;
    }
    [System.Serializable]
    public struct SpriteSendData
    {
        public SpriteRenderer spriteRenderer;
        public bool isMainSprite;
        public string propertyName;
    }

    [SerializeField]
    public CharaData data;
    [SerializeField]
    TextureSendData[] meshes = { };
    [SerializeField]
    SpriteSendData[] sprites = { };
    [SerializeField]
    Animator[] animators = { };
    [SerializeField]
    Transform[] bodyParts = { };

    public string animationName;
    public float animationProgress;
    public float animationSpeed;

    float prevAnimationProgress;
    string facialExpressionName;

    void FixedUpdate()
    {
        UpdateAnimation();

        animationProgress = Mathf.Repeat(animationProgress + animationSpeed, 1);
    }

    void UpdateAnimation()
    {
        //表情をリセット
        facialExpressionName = "";

        //パーツのトランスフォームをデフォルト値に揃える
        for (int i = 0; i < bodyParts.Length; i++)
        {
            Transform current = bodyParts[i];
            KX_netUtil.TransformData currentData =
                data.GetDefaultBodyPartsTransform(i);
            current.localPosition = currentData.position;
            current.localScale = currentData.scale;
            current.localEulerAngles = currentData.eulerAngles;
        }
        //現在の状態にあったアニメーションを取得
        CharaData.Animation animationData =
            data.SearchAnimation(animationName);
        //スピードを自動設定
        animationSpeed = 1f / Mathf.Max(1, animationData.totalFrame);

        //トランスフォームアニメーションを適用
        if (animationData.transformAnimationKeys != null)
        {
            for (int i = 0; i < animationData.transformAnimationKeys.Length; i++)
            {
                CharaData.TransformAnimationKey tAnimData =
                    animationData.transformAnimationKeys[i];
                if (KX_netUtil.IsIntoRange(
                    animationProgress,
                    tAnimData.keyFrame.x, tAnimData.keyFrame.y,
                    false, false))
                {
                    Transform current = bodyParts[tAnimData.bodyPartIndex];
                    float animationPartProgress =
                        KX_netUtil.Ease(KX_netUtil.RangeMap(animationProgress,
                        tAnimData.keyFrame.x, tAnimData.keyFrame.y,
                        0, 1),
                        tAnimData.easeType, tAnimData.easePow);

                    if (!tAnimData.ignoreTransform.position)
                    {
                        current.localPosition = Vector3.Lerp(
                            tAnimData.startTransform.position,
                            tAnimData.endTransform.position,
                            animationPartProgress);
                    }

                    if (!tAnimData.ignoreTransform.rotation)
                    {
                        Quaternion rotate;
                        if (tAnimData.lerpAsEuler)
                        {
                            rotate = Quaternion.Euler(Vector3.Lerp(
                                tAnimData.startTransform.eulerAngles,
                                tAnimData.endTransform.eulerAngles,
                                animationPartProgress));
                        }
                        else
                        {
                            rotate = Quaternion.Slerp(
                                Quaternion.Euler(tAnimData.startTransform.eulerAngles),
                                Quaternion.Euler(tAnimData.endTransform.eulerAngles),
                                animationPartProgress);
                        }

                        current.localRotation = rotate;
                    }

                    if (!tAnimData.ignoreTransform.scale)
                    {
                        current.localScale = Vector3.Lerp(
                        tAnimData.startTransform.scale,
                        tAnimData.endTransform.scale,
                        animationPartProgress);
                    }
                }
            }
        }

        //スキンアニメーションを適用
        if (animationData.rigAnimationKeys != null)
        {
            for (int i = 0; i < animationData.rigAnimationKeys.Length; i++)
            {
                CharaData.RigAnimationKey rAnimData =
                    animationData.rigAnimationKeys[i];
                if (KX_netUtil.IsIntoRange(
                    animationProgress,
                    rAnimData.keyFrame.x, rAnimData.keyFrame.y,
                    false, false))
                {
                    Animator current = animators[rAnimData.animatorIndex];
                    float animationPartProgress =
                        KX_netUtil.RangeMap(animationProgress,
                        rAnimData.keyFrame.x, rAnimData.keyFrame.y,
                        rAnimData.animationRange.x, rAnimData.animationRange.y);

                    current.SetInteger(rAnimData.parameterName, rAnimData.rigAnimationID);
                    current.Play(current.GetNextAnimatorStateInfo(0).nameHash,
                    0, animationPartProgress);
                }
            }
        }

        //表情を適用
        if (animationData.facialExpressionKeys != null)
        {
            for (int i = 0; i < animationData.facialExpressionKeys.Length; i++)
            {
                CharaData.FacialExpressionKey fKeyData =
                    animationData.facialExpressionKeys[i];
                if (KX_netUtil.IsIntoRange(
                    animationProgress,
                    fKeyData.keyFrame.x, fKeyData.keyFrame.y,
                    false, false))
                {
                    facialExpressionName = fKeyData.facialExpressionName;
                }
            }
        }

        //効果音
        if (animationData.seKeys != null)
        {
            for (int i = 0; i < animationData.
                seKeys.Length; i++)
            {
                CharaData.SEKey current =
                    animationData.seKeys[i];

                float shiftedprevAnimationProgress = prevAnimationProgress;
                if (animationProgress < prevAnimationProgress)
                {
                    shiftedprevAnimationProgress -= 1;
                }

                if (KX_netUtil.IsIntoRange(
                    current.keyFrame,
                    shiftedprevAnimationProgress, animationProgress,
                    false, false)
                    && GetComponent<AudioSource>())
                {
                    GetComponent<AudioSource>().PlayOneShot(current.se);
                }
            }
        }

        prevAnimationProgress = animationProgress;

        //デフォルトのPropertySetterを読み込む
        KX_netUtil.PropertySetter[] defaultPropertySetters = data.GetDefaultPropertySetters();

        //デフォルトのスプライトをスプライトレンダラーに貼る
        for (int i = 0; i < sprites.Length; i++)
        {
            SpriteSendData current = sprites[i];
            if (current.isMainSprite)
            {
                current.spriteRenderer.sprite = data.GetDefaultSprite(i);
            }
            else
            {
                current.spriteRenderer.material.
                    SetTexture(current.propertyName, data.GetDefaultSprite(i).texture);
            }
        }

        //現在の状態にあった表情を取得
        CharaData.FacialExpression facialData =
            data.SearchFacialExpression(facialExpressionName);
        //表情のPropertyReplacerで上書き
        if (facialData.indexAndPropertyReplacers != null)
        {
            for (int i = 0; i < facialData.indexAndPropertyReplacers.Length; i++)
            {
                CharaData.IndexAndPropertyReplacer current =
                    facialData.indexAndPropertyReplacers[i];
                KX_netUtil.ReplacePropertySetter(
                    defaultPropertySetters[current.index], current.propertyReplacer);
            }
        }
        //表情のスプライトをスプライトレンダラーに貼る
        //TODO:この処理を最新の方式に更新する
        if (facialData.indexAndSprites != null)
        {
            for (int i = 0; i < facialData.indexAndSprites.Length; i++)
            {
                CharaData.IndexAndSprite spriteData = facialData.indexAndSprites[i];
                SpriteSendData current = sprites[spriteData.index];
                if (current.isMainSprite)
                {
                    current.spriteRenderer.sprite = spriteData.sprite;
                }
                else
                {
                    current.spriteRenderer.material.
                        SetTexture(current.propertyName, spriteData.sprite.texture);
                }
            }
        }

        //テクスチャをモデルに貼る
        for (int i = 0; i < meshes.Length; i++)
        {
            TextureSendData current = meshes[i];
            if (current.meshRenderer && current.meshRenderer.materials != null
                && current.meshRenderer.materials.Length > current.index
                && defaultPropertySetters.Length > i)
            {
                KX_netUtil.ApplyMaterialPropertySetter(
                    current.meshRenderer.materials[current.index],
                    defaultPropertySetters[i]);
            }
        }
    }
}