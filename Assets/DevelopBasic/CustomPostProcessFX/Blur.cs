using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

[System.Serializable]
[PostProcess(typeof(BlurRenderer), PostProcessEvent.BeforeStack, "Custom/Blur")]
public class Blur : PostProcessEffectSettings
{
    [Range(0, 0.1f)]
    public FloatParameter blurAmount = new FloatParameter{value = 0f};
    [Range(15,100)]
    public IntParameter sampleAmount = new IntParameter{value = 15};
    public override bool IsEnabledAndSupported(PostProcessRenderContext context)
    {
        return enabled.value && blurAmount > 0 && sampleAmount > 0;
    }
}
public sealed class BlurRenderer: PostProcessEffectRenderer<Blur>{
    public override void Render(PostProcessRenderContext context)
    {
        var sheet = context.propertySheets.Get(Shader.Find("Hidden/Custom/Blur"));
        sheet.properties.SetFloat("_BlurSize", settings.blurAmount);
        sheet.properties.SetInt("_Sample", settings.sampleAmount);

    //There might be better way to get temporary RT for using built-in postprocess effect
    // context.command.DrawRenderer()
        var tempTex = RenderTexture.GetTemporary(context.width, context.height);
        context.command.BlitFullscreenTriangle(context.source, tempTex, sheet, 0);
        context.command.BlitFullscreenTriangle(tempTex, context.destination, sheet, 1);
        RenderTexture.ReleaseTemporary(tempTex);
    }
}
