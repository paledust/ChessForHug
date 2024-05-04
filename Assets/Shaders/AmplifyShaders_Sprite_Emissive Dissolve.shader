// Upgrade NOTE: upgraded instancing buffer 'AmplifyShadersSpriteEmissiveDissolve' to new syntax.

// Made with Amplify Shader Editor v1.9.3.3
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "AmplifyShaders/Sprite EmissiveDissolve"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		[MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
		[PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
		[Toggle]_FlipX("FlipX", Float) = 0
		[Toggle]_UseRectUV("UseRectUV", Float) = 0
		[HDR]_FillColor("FillColor", Color) = (0,0,0,1)
		_NoiseTex("NoiseTex", 2D) = "white" {}
		_NoiseTiling1("NoiseTiling", Float) = 1
		_DissolvePanSpeed("DissolvePanSpeed", Vector) = (0,0,0,0)
		_NoiseSpeed1("NoiseSpeed", Vector) = (1,1,0,0)
		_NoiseMin1("NoiseMin", Float) = -1.25
		_NoiseMax1("NoiseMax", Float) = 1
		_DissolveSoftness("DissolveSoftness", Float) = 1
		_PatternScale("PatternScale", Float) = 1
		[KeywordEnum(Radius,TexBased)] _DissolveType("DissolveType", Float) = 0
		_RadiusCenterX("RadiusCenterX", Float) = 0.5
		_RadiusCenterY("RadiusCenterY", Float) = 0.5
		_DissolveRadius("DissolveRadius", Float) = 0.5
		_DissolveLength("DissolveLength", Float) = 0.1
		_DissolveGuideTex("DissolveGuideTex", 2D) = "white" {}
		[PerRendererData]_SpriteRect("SpriteRect", Vector) = (1,1,1,1)
		[KeywordEnum(XY,XZ)] _UV_Direction("UV_Direction", Float) = 0
		_Desaturation("Desaturation", Float) = 1
		_DistortTex("DistortTex", 2D) = "gray" {}
		_DistortStrength("DistortStrength", Float) = 0
		_DistortXSpeed("DistortXSpeed", Float) = 0
		_DistortYSpeed("DistortYSpeed", Float) = 0

	}

	SubShader
	{
		LOD 0

		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha
		
		
		Pass
		{
		CGPROGRAM
			
			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile _ PIXELSNAP_ON
			#pragma multi_compile _ ETC1_EXTERNAL_ALPHA
			#include "UnityCG.cginc"
			#include "UnityShaderVariables.cginc"
			#define ASE_NEEDS_FRAG_COLOR
			#pragma multi_compile_instancing
			#pragma shader_feature_local _DISSOLVETYPE_RADIUS _DISSOLVETYPE_TEXBASED
			#pragma shader_feature_local _UV_DIRECTION_XY _UV_DIRECTION_XZ


			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 texcoord  : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				float4 ase_texcoord1 : TEXCOORD1;
			};
			
			uniform fixed4 _Color;
			uniform float _EnableExternalAlpha;
			uniform sampler2D _MainTex;
			uniform sampler2D _AlphaTex;
			uniform float4 _FillColor;
			uniform sampler2D _DistortTex;
			uniform float _DistortXSpeed;
			uniform float _DistortYSpeed;
			uniform float _DistortStrength;
			float4 _MainTex_TexelSize;
			uniform float _FlipX;
			uniform float _UseRectUV;
			uniform float _DissolveSoftness;
			uniform sampler2D _NoiseTex;
			uniform float _PatternScale;
			uniform float2 _DissolvePanSpeed;
			uniform float _RadiusCenterX;
			uniform float _RadiusCenterY;
			uniform float _DissolveRadius;
			uniform float _DissolveLength;
			uniform sampler2D _DissolveGuideTex;
			uniform float _Desaturation;
			uniform float _NoiseMin1;
			uniform float _NoiseMax1;
			uniform float2 _NoiseSpeed1;
			uniform float _NoiseTiling1;
			UNITY_INSTANCING_BUFFER_START(AmplifyShadersSpriteEmissiveDissolve)
				UNITY_DEFINE_INSTANCED_PROP(float4, _DistortTex_ST)
#define _DistortTex_ST_arr AmplifyShadersSpriteEmissiveDissolve
				UNITY_DEFINE_INSTANCED_PROP(float4, _SpriteRect)
#define _SpriteRect_arr AmplifyShadersSpriteEmissiveDissolve
			UNITY_INSTANCING_BUFFER_END(AmplifyShadersSpriteEmissiveDissolve)

			
			v2f vert( appdata_t IN  )
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID(IN);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
				float3 ase_worldPos = mul(unity_ObjectToWorld, float4( (IN.vertex).xyz, 1 )).xyz;
				OUT.ase_texcoord1.xyz = ase_worldPos;
				
				
				//setting value to unused interpolator channels and avoid initialization warnings
				OUT.ase_texcoord1.w = 0;
				
				IN.vertex.xyz +=  float3(0,0,0) ; 
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				#ifdef PIXELSNAP_ON
				OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			fixed4 SampleSpriteTexture (float2 uv)
			{
				fixed4 color = tex2D (_MainTex, uv);

#if ETC1_EXTERNAL_ALPHA
				// get the color from an external texture (usecase: Alpha support for ETC1 on android)
				fixed4 alpha = tex2D (_AlphaTex, uv);
				color.a = lerp (color.a, alpha.r, _EnableExternalAlpha);
#endif //ETC1_EXTERNAL_ALPHA

				return color;
			}
			
			fixed4 frag(v2f IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				float2 appendResult190 = (float2(_DistortXSpeed , _DistortYSpeed));
				float4 _DistortTex_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_DistortTex_ST_arr, _DistortTex_ST);
				float2 uv_DistortTex = IN.texcoord.xy * _DistortTex_ST_Instance.xy + _DistortTex_ST_Instance.zw;
				float2 panner183 = ( 1.0 * _Time.y * appendResult190 + uv_DistortTex);
				float4 tex2DNode180 = tex2D( _DistortTex, panner183 );
				float2 appendResult181 = (float2(tex2DNode180.r , tex2DNode180.g));
				float2 uvDistort188 = ( (appendResult181*2.0 + -1.0) * _DistortStrength );
				float2 texCoord87 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float2 appendResult132 = (float2(_MainTex_TexelSize.z , _MainTex_TexelSize.w));
				float4 _SpriteRect_Instance = UNITY_ACCESS_INSTANCED_PROP(_SpriteRect_arr, _SpriteRect);
				float4 break21_g6 = _SpriteRect_Instance;
				float2 appendResult4_g6 = (float2(break21_g6.x , break21_g6.y));
				float2 appendResult6_g6 = (float2(break21_g6.z , break21_g6.w));
				float2 break9_g6 = ( ( ( IN.texcoord.xy * appendResult132 ) - appendResult4_g6 ) / appendResult6_g6 );
				float lerpResult12_g6 = lerp( break9_g6.x , ( 1.0 - break9_g6.x ) , _FlipX);
				float2 appendResult10_g6 = (float2(lerpResult12_g6 , break9_g6.y));
				float2 lerpResult138 = lerp( texCoord87 , appendResult10_g6 , _UseRectUV);
				float2 uv143 = ( uvDistort188 + lerpResult138 );
				float4 tex2DNode5 = tex2D( _MainTex, uv143 );
				float3 ase_worldPos = IN.ase_texcoord1.xyz;
				float2 appendResult90 = (float2(ase_worldPos.x , ase_worldPos.y));
				float2 appendResult168 = (float2(ase_worldPos.x , ase_worldPos.z));
				#if defined(_UV_DIRECTION_XY)
				float2 staticSwitch169 = appendResult90;
				#elif defined(_UV_DIRECTION_XZ)
				float2 staticSwitch169 = appendResult168;
				#else
				float2 staticSwitch169 = appendResult90;
				#endif
				float2 worldUV170 = staticSwitch169;
				float2 panner126 = ( 1.0 * _Time.y * _DissolvePanSpeed + worldUV170);
				float2 appendResult141 = (float2(_RadiusCenterX , _RadiusCenterY));
				float dissolveRadius151 = _DissolveRadius;
				float dissolveLength152 = _DissolveLength;
				float temp_output_21_0_g7 = dissolveLength152;
				float2 panner124 = ( 1.0 * _Time.y * _DissolvePanSpeed + ( worldUV170 * _PatternScale ));
				#if defined(_DISSOLVETYPE_RADIUS)
				float staticSwitch93 = ( tex2D( _NoiseTex, ( _PatternScale * panner126 ) ).r + -( ( length( ( uv143 - appendResult141 ) ) - ( dissolveRadius151 - temp_output_21_0_g7 ) ) / temp_output_21_0_g7 ) );
				#elif defined(_DISSOLVETYPE_TEXBASED)
				float staticSwitch93 = ( tex2D( _NoiseTex, panner124 ).r + ( ( tex2D( _DissolveGuideTex, uv143 ).r - ( dissolveRadius151 - dissolveLength152 ) ) / dissolveLength152 ) );
				#else
				float staticSwitch93 = ( tex2D( _NoiseTex, ( _PatternScale * panner126 ) ).r + -( ( length( ( uv143 - appendResult141 ) ) - ( dissolveRadius151 - temp_output_21_0_g7 ) ) / temp_output_21_0_g7 ) );
				#endif
				float smoothstepResult42 = smoothstep( 0.0 , _DissolveSoftness , staticSwitch93);
				float4 appendResult40 = (float4(( _FillColor * tex2DNode5 ).rgb , ( tex2DNode5.a * saturate( smoothstepResult42 ) )));
				float3 temp_output_120_0 = (appendResult40).xyz;
				float3 desaturateInitialColor175 = temp_output_120_0;
				float desaturateDot175 = dot( desaturateInitialColor175, float3( 0.299, 0.587, 0.114 ));
				float3 desaturateVar175 = lerp( desaturateInitialColor175, desaturateDot175.xxx, _Desaturation );
				float2 panner109 = ( 1.0 * _Time.y * _NoiseSpeed1 + ( worldUV170 * _NoiseTiling1 ));
				float smoothstepResult110 = smoothstep( _NoiseMin1 , _NoiseMax1 , tex2D( _NoiseTex, panner109 ).r);
				float noisePattern117 = smoothstepResult110;
				float3 lerpResult176 = lerp( temp_output_120_0 , desaturateVar175 , noisePattern117);
				float4 appendResult122 = (float4(lerpResult176 , (appendResult40).w));
				
				fixed4 c = ( appendResult122 * IN.color );
				c.rgb *= c.a;
				return c;
			}
		ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	Fallback Off
}
/*ASEBEGIN
Version=19303
Node;AmplifyShaderEditor.RangedFloatNode;184;-5328,1184;Inherit;False;Property;_DistortXSpeed;DistortXSpeed;23;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;189;-5328,1264;Inherit;False;Property;_DistortYSpeed;DistortYSpeed;24;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;182;-5184,1040;Inherit;False;0;180;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;190;-5120,1216;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;183;-4800,1152;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;180;-4544,1040;Inherit;True;Property;_DistortTex;DistortTex;21;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;gray;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;181;-4208,1056;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;162;-5296,-528;Inherit;False;0;0;_MainTex;Shader;False;0;5;SAMPLER2D;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScaleAndOffsetNode;185;-4064,1056;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT;2;False;2;FLOAT;-1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;187;-4016,1200;Inherit;False;Property;_DistortStrength;DistortStrength;22;0;Create;True;0;0;0;False;0;False;0;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexelSizeNode;131;-4960,-688;Inherit;False;-1;1;0;SAMPLER2D;;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;186;-3808,1056;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;132;-4784,-608;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector4Node;133;-4800,-464;Inherit;False;InstancedProperty;_SpriteRect;SpriteRect;18;1;[PerRendererData];Create;True;0;0;0;False;0;False;1,1,1,1;1,1,1,1;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldPosInputsNode;89;-4611.283,-1650.551;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldPosInputsNode;167;-4610.568,-1443.154;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RegisterLocalVarNode;188;-3648,1056;Inherit;False;uvDistort;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;137;-4336,-592;Inherit;False;Property;_UseRectUV;UseRectUV;2;1;[Toggle];Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;127;-4592,-608;Inherit;False;SRectUV;0;;6;8a9219883f02c404d93cd88cc4103060;0;2;16;FLOAT2;0,0;False;20;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;87;-4368,-720;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;90;-4424.589,-1626.174;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;168;-4423.874,-1418.777;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;138;-4080,-640;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;191;-4144,-800;Inherit;False;188;uvDistort;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-4996.393,159.066;Inherit;False;Property;_DissolveRadius;DissolveRadius;15;0;Create;True;0;0;0;False;0;False;0.5;0.62;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;26;-4958.093,238.4658;Inherit;False;Property;_DissolveLength;DissolveLength;16;0;Create;True;0;0;0;False;0;False;0.1;0.16;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;169;-4207.823,-1492.956;Inherit;False;Property;_UV_Direction;UV_Direction;19;0;Create;True;0;0;0;False;0;False;0;0;0;True;;KeywordEnum;2;XY;XZ;Create;True;True;All;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;192;-3904,-704;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;94;-3387.627,-1255.662;Inherit;False;1354.282;733.9672;Comment;8;157;154;153;86;126;81;35;171;Radius Dissolve;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;151;-4755.904,160.8437;Inherit;False;dissolveRadius;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;152;-4733.191,241.2049;Inherit;False;dissolveLength;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;170;-3844.982,-1492.656;Inherit;False;worldUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;143;-3760,-704;Inherit;False;uv;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;35;-2616.896,-1007.878;Inherit;False;Property;_PatternScale;PatternScale;11;0;Create;True;0;0;0;False;0;False;1;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;144;-3231.391,290.8321;Inherit;False;143;uv;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;155;-3121.179,594.422;Inherit;False;152;dissolveLength;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;156;-3138.179,467.4219;Inherit;False;151;dissolveRadius;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;173;-3093.674,-221.3472;Inherit;False;170;worldUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;98;-2866.725,-175.4979;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;139;-3495.956,-433.099;Inherit;False;Property;_RadiusCenterX;RadiusCenterX;13;0;Create;True;0;0;0;False;0;False;0.5;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;140;-3489.956,-352.099;Inherit;False;Property;_RadiusCenterY;RadiusCenterY;14;0;Create;True;0;0;0;False;0;False;0.5;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;148;-2863.3,496.7604;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;101;-3011.913,264.7142;Inherit;True;Property;_DissolveGuideTex;DissolveGuideTex;17;0;Create;True;0;0;0;False;0;False;-1;None;4a2db40eb501e29448a4a2719e6dd55c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;125;-2912,-32;Inherit;False;Property;_DissolvePanSpeed;DissolvePanSpeed;6;0;Create;True;0;0;0;False;0;False;0,0;0.2,-0.5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;124;-2658.49,-60.9002;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.2,0.5;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;141;-3311.956,-409.099;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;147;-2644.507,409.5522;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;171;-2877.148,-989.1553;Inherit;False;170;worldUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;81;-2663.767,-1205.662;Inherit;True;Property;_NoiseTex;NoiseTex;4;0;Create;True;0;0;0;False;0;False;None;c4c49450695bc524090ebf9ba7e9ca6e;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SamplerNode;99;-2431.665,-202.8896;Inherit;True;Property;_TextureSample0;Texture Sample 0;13;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;126;-2599.434,-926.1683;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.2,0.5;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;86;-3037.334,-775.61;Inherit;True;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;153;-2310.35,-722.5093;Inherit;False;151;dissolveRadius;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;154;-2306.35,-641.5093;Inherit;False;152;dissolveLength;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;145;-2434.809,573.3646;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;116;-1802.621,-1648.134;Inherit;False;1803.927;660.815;Comment;10;114;111;110;109;108;107;106;105;117;172;NoisePattern;1,1,1,1;0;0
Node;AmplifyShaderEditor.FunctionNode;157;-2307.345,-934.6534;Inherit;False;SRadiusDissolve;-1;;7;a318621bd65470b43ac73e4fd821eb8a;0;6;22;FLOAT2;0,0;False;17;SAMPLER2D;;False;18;FLOAT;1;False;19;FLOAT2;0,0;False;20;FLOAT;0.5;False;21;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;158;-2065.331,-13.0172;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;107;-1605.373,-1438.346;Inherit;False;Property;_NoiseTiling1;NoiseTiling;5;0;Create;True;0;0;0;False;0;False;1;0.1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;172;-1613.103,-1564.29;Inherit;False;170;worldUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StaticSwitch;93;-1776.477,-544.877;Inherit;False;Property;_DissolveType;DissolveType;12;0;Create;True;0;0;0;False;0;False;0;0;0;True;;KeywordEnum;2;Radius;TexBased;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;43;-1039.924,-167.9791;Inherit;False;Property;_DissolveSoftness;DissolveSoftness;10;0;Create;True;0;0;0;False;0;False;1;1.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;105;-1409.373,-1513.346;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;108;-1439.63,-1362.362;Inherit;False;Property;_NoiseSpeed1;NoiseSpeed;7;0;Create;True;0;0;0;False;0;False;1,1;0.2,-0.02;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SmoothstepOpNode;42;-813.7785,-235.7707;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;193;-1056,-416;Inherit;False;143;uv;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;109;-1248.63,-1512.362;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.2,0.5;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;5;-828.062,-495.7421;Inherit;True;Property;_test;test;0;0;Create;True;0;0;0;False;0;False;-1;7a170cdb7cc88024cb628cfcdbb6705c;7a170cdb7cc88024cb628cfcdbb6705c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;79;-770.9401,-679.2653;Inherit;False;Property;_FillColor;FillColor;3;1;[HDR];Create;True;0;0;0;False;0;False;0,0,0,1;1,1,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;41;-610.0541,-237.8656;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;111;-916.1506,-1347.626;Inherit;False;Property;_NoiseMin1;NoiseMin;8;0;Create;True;0;0;0;False;0;False;-1.25;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;114;-947.1506,-1272.626;Inherit;False;Property;_NoiseMax1;NoiseMax;9;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;106;-1055.47,-1536.778;Inherit;True;Property;_NoiseMask1;NoiseMask;2;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;78;-486.3017,-599.6647;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;-304,-256;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;110;-714.1506,-1368.626;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;40;-32,-416;Inherit;False;FLOAT4;4;0;FLOAT3;1,0,0;False;1;FLOAT;1;False;2;FLOAT;1;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.ComponentMaskNode;120;192,-624;Inherit;False;True;True;True;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;177;226.5935,-519.1523;Inherit;False;Property;_Desaturation;Desaturation;20;0;Create;True;0;0;0;False;0;False;1;0.6;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;117;-435.785,-1361.999;Inherit;False;noisePattern;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;118;208,-432;Inherit;False;117;noisePattern;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.DesaturateOpNode;175;416,-560;Inherit;False;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;121;208,-320;Inherit;False;False;False;False;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;176;656,-624;Inherit;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;122;832,-432;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.VertexColorNode;179;929.8684,-276.5215;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;178;1057.068,-462.1216;Inherit;False;2;2;0;FLOAT4;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;166;1296,-464;Float;False;True;-1;2;ASEMaterialInspector;0;10;AmplifyShaders/Sprite EmissiveDissolve;0f8ba0101102bb14ebf021ddadce9b49;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;2;False;True;3;1;False;;10;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;190;0;184;0
WireConnection;190;1;189;0
WireConnection;183;0;182;0
WireConnection;183;2;190;0
WireConnection;180;1;183;0
WireConnection;181;0;180;1
WireConnection;181;1;180;2
WireConnection;185;0;181;0
WireConnection;131;0;162;0
WireConnection;186;0;185;0
WireConnection;186;1;187;0
WireConnection;132;0;131;3
WireConnection;132;1;131;4
WireConnection;188;0;186;0
WireConnection;127;16;132;0
WireConnection;127;20;133;0
WireConnection;90;0;89;1
WireConnection;90;1;89;2
WireConnection;168;0;167;1
WireConnection;168;1;167;3
WireConnection;138;0;87;0
WireConnection;138;1;127;0
WireConnection;138;2;137;0
WireConnection;169;1;90;0
WireConnection;169;0;168;0
WireConnection;192;0;191;0
WireConnection;192;1;138;0
WireConnection;151;0;25;0
WireConnection;152;0;26;0
WireConnection;170;0;169;0
WireConnection;143;0;192;0
WireConnection;98;0;173;0
WireConnection;98;1;35;0
WireConnection;148;0;156;0
WireConnection;148;1;155;0
WireConnection;101;1;144;0
WireConnection;124;0;98;0
WireConnection;124;2;125;0
WireConnection;141;0;139;0
WireConnection;141;1;140;0
WireConnection;147;0;101;1
WireConnection;147;1;148;0
WireConnection;99;0;81;0
WireConnection;99;1;124;0
WireConnection;126;0;171;0
WireConnection;126;2;125;0
WireConnection;86;0;143;0
WireConnection;86;1;141;0
WireConnection;145;0;147;0
WireConnection;145;1;155;0
WireConnection;157;22;126;0
WireConnection;157;17;81;0
WireConnection;157;18;35;0
WireConnection;157;19;86;0
WireConnection;157;20;153;0
WireConnection;157;21;154;0
WireConnection;158;0;99;1
WireConnection;158;1;145;0
WireConnection;93;1;157;0
WireConnection;93;0;158;0
WireConnection;105;0;172;0
WireConnection;105;1;107;0
WireConnection;42;0;93;0
WireConnection;42;2;43;0
WireConnection;109;0;105;0
WireConnection;109;2;108;0
WireConnection;5;0;162;0
WireConnection;5;1;193;0
WireConnection;41;0;42;0
WireConnection;106;0;81;0
WireConnection;106;1;109;0
WireConnection;78;0;79;0
WireConnection;78;1;5;0
WireConnection;39;0;5;4
WireConnection;39;1;41;0
WireConnection;110;0;106;1
WireConnection;110;1;111;0
WireConnection;110;2;114;0
WireConnection;40;0;78;0
WireConnection;40;3;39;0
WireConnection;120;0;40;0
WireConnection;117;0;110;0
WireConnection;175;0;120;0
WireConnection;175;1;177;0
WireConnection;121;0;40;0
WireConnection;176;0;120;0
WireConnection;176;1;175;0
WireConnection;176;2;118;0
WireConnection;122;0;176;0
WireConnection;122;3;121;0
WireConnection;178;0;122;0
WireConnection;178;1;179;0
WireConnection;166;0;178;0
ASEEND*/
//CHKSM=093F83957E544375275A15B60D645F7443823D9E