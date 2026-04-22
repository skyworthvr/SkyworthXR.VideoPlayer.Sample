Shader "Unlit/SvrLaser"
{
    Properties
    {
    }
    
    SubShader
    {
        Tags 
        {
            "IgnoreProjector" = "True"
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
        }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            CGPROGRAM
            #pragma vertex vert  
            #pragma fragment frag
 
            #include "UnityCG.cginc"
 
            struct v2f{
                float4 pos:POSITION;
                float4 col:COLOR;
            };
 
            v2f vert(appdata_full v){
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.col = v.color;
                o.col = clamp(o.col, 0.0, 1.0);
                return o;
            }
         
            float4 frag(v2f IN):COLOR{
                //float a = min(col.a * 2.0, 1);
                //a = max(a - 0.5, 0) * 2;
                return IN.col;;
            }
            
            ENDCG
        }
    }
}
