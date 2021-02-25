Shader "Unlit/ChromaticAberration"
{
    Properties {
        _MainTex ("Base (RGB)", 2D) = "white" {}
    }
    SubShader
    {
        Cull Off
        Lighting Off
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
       
        CGPROGRAM
        #pragma vertex vert_img
        #pragma fragment frag
        #pragma fragmentoption ARB_precision_hint_fastest
        #include "UnityCG.cginc"
 
        uniform sampler2D _MainTex;
        uniform float _AberrationOffsetX;
        uniform float _AberrationOffsetY;
 
        float4 frag(v2f_img i) : COLOR
        {
            float2 coords = i.uv.xy;
           
            _AberrationOffsetX /= 300.0f;
            _AberrationOffsetY /= 300.0f;
           
            //Red Channel
            float4 red = tex2D(_MainTex , float2(coords.x - _AberrationOffsetX, coords.y + _AberrationOffsetY));
            //Green Channel
            float4 green = tex2D(_MainTex, float2(coords.x + _AberrationOffsetX, coords.y - _AberrationOffsetY));
            //Blue Channel
            float4 blue = tex2D(_MainTex, coords.xy);
           
            float4 finalColor = float4(red.r, green.g, blue.b, 1.0f);
            return finalColor;
        }
 
        ENDCG
        }
    }
}