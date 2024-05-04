// Upgrade NOTE: upgraded instancing buffer 'AmplifyShadersSpriteEmissiveDissolve' to new syntax.

// Made with Amplify Shader Editor v1.9.2.1
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
		_EdgeValue("EdgeValue", Float) = 0
		_EdgeSoftness("EdgeSoftness", Float) = 1
		[HDR]_EdgeColor("EdgeColor", Color) = (2.828427,2.029812,1.209014,1)
		[KeywordEnum(AlphaBlend,Additive)] _EdgeBlend("EdgeBlend", Float) = 0
		[PerRendererData]_SpriteRect("SpriteRect", Vector) = (1,1,1,1)
		[KeywordEnum(XY,XZ)] _UV_Direction("UV_Direction", Float) = 0
		[Toggle(_USEFLICKERING_ON)] _UseFlickering("Use Flickering", Float) = 0
		_SpriteFlickering("SpriteFlickering", Float) = 1
		_FlickeringOffset("FlickeringOffset", Float) = 1
		_FlickeringMin("FlickeringMin", Float) = 0
		_FlickeringMax("FlickeringMax", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

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
			#pragma shader_feature_local _UV_DIRECTION_XY _UV_DIRECTION_XZ
			#pragma shader_feature_local _EDGEBLEND_ALPHABLEND _EDGEBLEND_ADDITIVE
			#pragma multi_compile_instancing
			#pragma shader_feature_local _DISSOLVETYPE_RADIUS _DISSOLVETYPE_TEXBASED
			#pragma shader_feature_local _USEFLICKERING_ON


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
			uniform float _NoiseMin1;
			uniform float _NoiseMax1;
			uniform sampler2D _NoiseTex;
			uniform float2 _NoiseSpeed1;
			uniform float _NoiseTiling1;
			uniform float4 _FillColor;
			uniform float4 _EdgeColor;
			uniform float _DissolveSoftness;
			uniform float _PatternScale;
			uniform float2 _DissolvePanSpeed;
			float4 _MainTex_TexelSize;
			uniform float _FlipX;
			uniform float _UseRectUV;
			uniform float _RadiusCenterX;
			uniform float _RadiusCenterY;
			uniform float _DissolveRadius;
			uniform float _DissolveLength;
			uniform sampler2D _DissolveGuideTex;
			uniform float _EdgeValue;
			uniform float _EdgeSoftness;
			uniform float _FlickeringMin;
			uniform float _FlickeringMax;
			uniform float _FlickeringOffset;
			uniform float _SpriteFlickering;
			UNITY_INSTANCING_BUFFER_START(AmplifyShadersSpriteEmissiveDissolve)
				UNITY_DEFINE_INSTANCED_PROP(float4, _MainTex_ST)
#define _MainTex_ST_arr AmplifyShadersSpriteEmissiveDissolve
				UNITY_DEFINE_INSTANCED_PROP(float4, _SpriteRect)
#define _SpriteRect_arr AmplifyShadersSpriteEmissiveDissolve
			UNITY_INSTANCING_BUFFER_END(AmplifyShadersSpriteEmissiveDissolve)
			float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }
			float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }
			float snoise( float2 v )
			{
				const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
				float2 i = floor( v + dot( v, C.yy ) );
				float2 x0 = v - i + dot( i, C.xx );
				float2 i1;
				i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
				float4 x12 = x0.xyxy + C.xxzz;
				x12.xy -= i1;
				i = mod2D289( i );
				float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
				float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
				m = m * m;
				m = m * m;
				float3 x = 2.0 * frac( p * C.www ) - 1.0;
				float3 h = abs( x ) - 0.5;
				float3 ox = floor( x + 0.5 );
				float3 a0 = x - ox;
				m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
				float3 g;
				g.x = a0.x * x0.x + h.x * x0.y;
				g.yz = a0.yz * x12.xz + h.yz * x12.yw;
				return 130.0 * dot( m, g );
			}
			

			
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
				float2 panner109 = ( 1.0 * _Time.y * _NoiseSpeed1 + ( worldUV170 * _NoiseTiling1 ));
				float smoothstepResult110 = smoothstep( _NoiseMin1 , _NoiseMax1 , tex2D( _NoiseTex, panner109 ).r);
				float noisePattern117 = smoothstepResult110;
				float4 _MainTex_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_MainTex_ST_arr, _MainTex_ST);
				float2 uv_MainTex = IN.texcoord.xy * _MainTex_ST_Instance.xy + _MainTex_ST_Instance.zw;
				float4 tex2DNode5 = tex2D( _MainTex, uv_MainTex );
				float4 temp_output_78_0 = ( _FillColor * tex2DNode5 );
				float2 panner126 = ( 1.0 * _Time.y * _DissolvePanSpeed + worldUV170);
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
				float2 uv143 = lerpResult138;
				float2 appendResult141 = (float2(_RadiusCenterX , _RadiusCenterY));
				float dissolveRadius151 = _DissolveRadius;
				float dissolveLength152 = _DissolveLength;
				float temp_output_21_0_g5 = dissolveLength152;
				float2 panner124 = ( 1.0 * _Time.y * _DissolvePanSpeed + ( worldUV170 * _PatternScale ));
				#if defined(_DISSOLVETYPE_RADIUS)
				float staticSwitch93 = ( tex2D( _NoiseTex, ( _PatternScale * panner126 ) ).r + -( ( length( ( uv143 - appendResult141 ) ) - ( dissolveRadius151 - temp_output_21_0_g5 ) ) / temp_output_21_0_g5 ) );
				#elif defined(_DISSOLVETYPE_TEXBASED)
				float staticSwitch93 = ( tex2D( _NoiseTex, panner124 ).r + ( ( tex2D( _DissolveGuideTex, uv143 ).r - ( dissolveRadius151 - dissolveLength152 ) ) / dissolveLength152 ) );
				#else
				float staticSwitch93 = ( tex2D( _NoiseTex, ( _PatternScale * panner126 ) ).r + -( ( length( ( uv143 - appendResult141 ) ) - ( dissolveRadius151 - temp_output_21_0_g5 ) ) / temp_output_21_0_g5 ) );
				#endif
				float smoothstepResult42 = smoothstep( 0.0 , _DissolveSoftness , staticSwitch93);
				float temp_output_41_0 = saturate( smoothstepResult42 );
				float smoothstepResult49 = smoothstep( _EdgeValue , ( _EdgeValue + _EdgeSoftness ) , staticSwitch93);
				float temp_output_59_0 = saturate( ( temp_output_41_0 * ( 1.0 - smoothstepResult49 ) ) );
				float4 lerpResult91 = lerp( temp_output_78_0 , _EdgeColor , temp_output_59_0);
				#if defined(_EDGEBLEND_ALPHABLEND)
				float4 staticSwitch92 = lerpResult91;
				#elif defined(_EDGEBLEND_ADDITIVE)
				float4 staticSwitch92 = ( temp_output_78_0 + ( temp_output_59_0 * _EdgeColor ) );
				#else
				float4 staticSwitch92 = lerpResult91;
				#endif
				float4 appendResult40 = (float4(staticSwitch92.rgb , ( tex2DNode5.a * temp_output_41_0 )));
				float mulTime177 = _Time.y * _SpriteFlickering;
				float2 temp_cast_1 = (( _FlickeringOffset + mulTime177 )).xx;
				float simplePerlin2D178 = snoise( temp_cast_1*5.0 );
				simplePerlin2D178 = simplePerlin2D178*0.5 + 0.5;
				float smoothstepResult179 = smoothstep( _FlickeringMin , _FlickeringMax , simplePerlin2D178);
				#ifdef _USEFLICKERING_ON
				float staticSwitch175 = smoothstepResult179;
				#else
				float staticSwitch175 = 1.0;
				#endif
				float spriteFlickering182 = staticSwitch175;
				float4 appendResult122 = (float4(( noisePattern117 * (appendResult40).xyz ) , ( (appendResult40).w * spriteFlickering182 )));
				
				fixed4 c = ( IN.color * appendResult122 );
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
Version=19201
Node;AmplifyShaderEditor.CommentaryNode;116;-1802.621,-1648.134;Inherit;False;1803.927;660.815;Comment;10;114;111;110;109;108;107;106;105;117;172;NoisePattern;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;94;-3387.627,-1255.662;Inherit;False;1354.282;733.9672;Comment;8;157;154;153;86;126;81;35;171;Radius Dissolve;1,1,1,1;0;0
Node;AmplifyShaderEditor.SmoothstepOpNode;49;-883.8997,65.50784;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;53;-668.2249,66.04282;Inherit;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;52;-1389.426,149.443;Inherit;False;Property;_EdgeSoftness;EdgeSoftness;19;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;57;-1103.601,124.47;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;51;-1358.425,76.44285;Inherit;False;Property;_EdgeValue;EdgeValue;18;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;39;-297.0995,-263.5509;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;59;-332.6537,-34.4746;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;78;-486.3017,-599.6647;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;41;-610.0541,-237.8656;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;-476.1532,-34.67459;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;54;-370.3588,172.0972;Inherit;False;Property;_EdgeColor;EdgeColor;20;1;[HDR];Create;True;0;0;0;False;0;False;2.828427,2.029812,1.209014,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LerpOp;91;226.4028,-137.1306;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;56;154.945,-584.6371;Inherit;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DynamicAppendNode;40;992.0846,-317.2522;Inherit;False;FLOAT4;4;0;FLOAT3;1,0,0;False;1;FLOAT;1;False;2;FLOAT;1;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.StaticSwitch;92;718.0056,-337.5866;Inherit;False;Property;_EdgeBlend;EdgeBlend;21;0;Create;True;0;0;0;False;0;False;0;0;0;True;;KeywordEnum;2;AlphaBlend;Additive;Create;True;True;All;9;1;COLOR;0,0,0,0;False;0;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;4;COLOR;0,0,0,0;False;5;COLOR;0,0,0,0;False;6;COLOR;0,0,0,0;False;7;COLOR;0,0,0,0;False;8;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StaticSwitch;93;-1776.477,-544.877;Inherit;False;Property;_DissolveType;DissolveType;12;0;Create;True;0;0;0;False;0;False;0;0;0;True;;KeywordEnum;2;Radius;TexBased;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;35;-2616.896,-1007.878;Inherit;False;Property;_PatternScale;PatternScale;11;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;98;-2866.725,-175.4979;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;105;-1409.373,-1513.346;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SmoothstepOpNode;110;-714.1506,-1368.626;Inherit;True;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;111;-916.1506,-1347.626;Inherit;False;Property;_NoiseMin1;NoiseMin;8;0;Create;True;0;0;0;False;0;False;-1.25;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;114;-947.1506,-1272.626;Inherit;False;Property;_NoiseMax1;NoiseMax;9;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;99;-2431.665,-202.8896;Inherit;True;Property;_TextureSample0;Texture Sample 0;13;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;81;-2663.767,-1205.662;Inherit;True;Property;_NoiseTex;NoiseTex;4;0;Create;True;0;0;0;False;0;False;None;None;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.SamplerNode;106;-1055.47,-1536.778;Inherit;True;Property;_NoiseMask1;NoiseMask;2;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;107;-1605.373,-1438.346;Inherit;False;Property;_NoiseTiling1;NoiseTiling;5;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;117;-435.785,-1361.999;Inherit;False;noisePattern;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;120;1172.593,-383.2383;Inherit;False;True;True;True;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ComponentMaskNode;121;1186.608,-222.857;Inherit;False;False;False;False;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;118;1183.636,-464.3752;Inherit;False;117;noisePattern;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;123;1398.373,-437.7367;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.DynamicAppendNode;122;1591.454,-334.9681;Inherit;False;FLOAT4;4;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.PannerNode;109;-1248.63,-1512.362;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.2,0.5;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector2Node;108;-1439.63,-1362.362;Inherit;False;Property;_NoiseSpeed1;NoiseSpeed;7;0;Create;True;0;0;0;False;0;False;1,1;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;125;-2885.366,11.54383;Inherit;False;Property;_DissolvePanSpeed;DissolvePanSpeed;6;0;Create;True;0;0;0;False;0;False;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;124;-2658.49,-60.9002;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.2,0.5;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;126;-2599.434,-926.1683;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.2,0.5;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;5;-828.062,-495.7421;Inherit;True;Property;_test;test;0;0;Create;True;0;0;0;False;0;False;-1;7a170cdb7cc88024cb628cfcdbb6705c;7a170cdb7cc88024cb628cfcdbb6705c;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;86;-3037.334,-775.61;Inherit;True;2;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;-15.49266,145.7077;Inherit;True;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;139;-3495.956,-433.099;Inherit;False;Property;_RadiusCenterX;RadiusCenterX;13;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;140;-3489.956,-352.099;Inherit;False;Property;_RadiusCenterY;RadiusCenterY;14;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;141;-3311.956,-409.099;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexelSizeNode;131;-4747.739,-678.9379;Inherit;False;-1;1;0;SAMPLER2D;;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DynamicAppendNode;132;-4565.736,-606.9378;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.LerpOp;138;-3867.3,-638.0725;Inherit;False;3;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;143;-3706.556,-638.9611;Inherit;False;uv;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;43;-1039.924,-167.9791;Inherit;False;Property;_DissolveSoftness;DissolveSoftness;10;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;153;-2310.35,-722.5093;Inherit;False;151;dissolveRadius;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;154;-2306.35,-641.5093;Inherit;False;152;dissolveLength;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;25;-4996.393,159.066;Inherit;False;Property;_DissolveRadius;DissolveRadius;15;0;Create;True;0;0;0;False;0;False;0.5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;26;-4958.093,238.4658;Inherit;False;Property;_DissolveLength;DissolveLength;16;0;Create;True;0;0;0;False;0;False;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;151;-4755.904,160.8437;Inherit;False;dissolveRadius;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;144;-3231.391,290.8321;Inherit;False;143;uv;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;155;-3121.179,594.422;Inherit;False;152;dissolveLength;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;156;-3138.179,467.4219;Inherit;False;151;dissolveRadius;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;148;-2863.3,496.7604;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;145;-2434.809,573.3646;Inherit;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;147;-2644.507,409.5522;Inherit;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FunctionNode;157;-2307.345,-934.6534;Inherit;False;SRadiusDissolve;-1;;5;a318621bd65470b43ac73e4fd821eb8a;0;6;22;FLOAT2;0,0;False;17;SAMPLER2D;;False;18;FLOAT;1;False;19;FLOAT2;0,0;False;20;FLOAT;0.5;False;21;FLOAT;0.1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;158;-2065.331,-13.0172;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;152;-4733.191,241.2049;Inherit;False;dissolveLength;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;42;-813.7785,-235.7707;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;101;-3011.913,264.7142;Inherit;True;Property;_DissolveGuideTex;DissolveGuideTex;17;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;79;-770.9401,-679.2653;Inherit;False;Property;_FillColor;FillColor;3;1;[HDR];Create;True;0;0;0;False;0;False;0,0,0,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;164;1763.782,-436.2062;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT4;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.VertexColorNode;165;1558.782,-731.2062;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TemplateShaderPropertyNode;162;-5084.323,-525.2783;Inherit;False;0;0;_MainTex;Shader;False;0;5;SAMPLER2D;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector4Node;133;-4590.742,-466.0577;Inherit;False;InstancedProperty;_SpriteRect;SpriteRect;22;1;[PerRendererData];Create;True;0;0;0;False;0;False;1,1,1,1;0,0,0,0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FunctionNode;127;-4372.401,-607.4377;Inherit;False;SRectUV;0;;6;8a9219883f02c404d93cd88cc4103060;0;2;16;FLOAT2;0,0;False;20;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;137;-4121.498,-587.6378;Inherit;False;Property;_UseRectUV;UseRectUV;2;1;[Toggle];Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;87;-4151.442,-711.9469;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldPosInputsNode;89;-4611.283,-1650.551;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;90;-4424.589,-1626.174;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.WorldPosInputsNode;167;-4610.568,-1443.154;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;168;-4423.874,-1418.777;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.StaticSwitch;169;-4207.823,-1492.956;Inherit;False;Property;_UV_Direction;UV_Direction;23;0;Create;True;0;0;0;False;0;False;0;0;0;True;;KeywordEnum;2;XY;XZ;Create;True;True;All;9;1;FLOAT2;0,0;False;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT2;0,0;False;6;FLOAT2;0,0;False;7;FLOAT2;0,0;False;8;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;170;-3844.982,-1492.656;Inherit;False;worldUV;-1;True;1;0;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;171;-2877.148,-989.1553;Inherit;False;170;worldUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;172;-1613.103,-1564.29;Inherit;False;170;worldUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.GetLocalVarNode;173;-3093.674,-221.3472;Inherit;False;170;worldUV;1;0;OBJECT;;False;1;FLOAT2;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;178;-5141.081,-2023.2;Inherit;True;Simplex2D;True;False;2;0;FLOAT2;0,0;False;1;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;179;-4779.081,-1812.2;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;181;-5391.081,-1727.2;Inherit;False;Property;_FlickeringMax;FlickeringMax;28;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;180;-5342.081,-1808.2;Inherit;False;Property;_FlickeringMin;FlickeringMin;27;0;Create;True;0;0;0;False;0;False;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;176;-4651.044,-1978.258;Inherit;False;Constant;_Float0;Float 0;23;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;183;1155.975,-105.1372;Inherit;False;182;spriteFlickering;1;0;OBJECT;;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;184;1417.68,-193.706;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StaticSwitch;175;-4388.097,-1847.258;Inherit;False;Property;_UseFlickering;Use Flickering;24;0;Create;True;0;0;0;False;0;False;0;0;0;True;;Toggle;2;Key0;Key1;Create;True;True;All;9;1;FLOAT;0;False;0;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT;0;False;7;FLOAT;0;False;8;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;182;-4091.895,-1847.734;Inherit;False;spriteFlickering;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;166;2499.532,-439.5993;Float;False;True;-1;2;ASEMaterialInspector;0;10;AmplifyShaders/Sprite EmissiveDissolve;0f8ba0101102bb14ebf021ddadce9b49;True;SubShader 0 Pass 0;0;0;SubShader 0 Pass 0;2;False;True;3;1;False;;10;False;;0;1;False;;0;False;;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;False;False;False;False;False;False;False;False;False;True;2;False;;False;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;0;;0;0;Standard;0;0;1;True;False;;False;0
Node;AmplifyShaderEditor.RangedFloatNode;185;-5524.689,-2135.631;Inherit;False;Property;_FlickeringOffset;FlickeringOffset;26;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;177;-5556.88,-2023.392;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;174;-5752.587,-2023.423;Inherit;False;Property;_SpriteFlickering;SpriteFlickering;25;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;186;-5347.043,-2053.366;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
WireConnection;49;0;93;0
WireConnection;49;1;51;0
WireConnection;49;2;57;0
WireConnection;53;0;49;0
WireConnection;57;0;51;0
WireConnection;57;1;52;0
WireConnection;39;0;5;4
WireConnection;39;1;41;0
WireConnection;59;0;58;0
WireConnection;78;0;79;0
WireConnection;78;1;5;0
WireConnection;41;0;42;0
WireConnection;58;0;41;0
WireConnection;58;1;53;0
WireConnection;91;0;78;0
WireConnection;91;1;54;0
WireConnection;91;2;59;0
WireConnection;56;0;78;0
WireConnection;56;1;55;0
WireConnection;40;0;92;0
WireConnection;40;3;39;0
WireConnection;92;1;91;0
WireConnection;92;0;56;0
WireConnection;93;1;157;0
WireConnection;93;0;158;0
WireConnection;98;0;173;0
WireConnection;98;1;35;0
WireConnection;105;0;172;0
WireConnection;105;1;107;0
WireConnection;110;0;106;1
WireConnection;110;1;111;0
WireConnection;110;2;114;0
WireConnection;99;0;81;0
WireConnection;99;1;124;0
WireConnection;106;0;81;0
WireConnection;106;1;109;0
WireConnection;117;0;110;0
WireConnection;120;0;40;0
WireConnection;121;0;40;0
WireConnection;123;0;118;0
WireConnection;123;1;120;0
WireConnection;122;0;123;0
WireConnection;122;3;184;0
WireConnection;109;0;105;0
WireConnection;109;2;108;0
WireConnection;124;0;98;0
WireConnection;124;2;125;0
WireConnection;126;0;171;0
WireConnection;126;2;125;0
WireConnection;5;0;162;0
WireConnection;86;0;143;0
WireConnection;86;1;141;0
WireConnection;55;0;59;0
WireConnection;55;1;54;0
WireConnection;141;0;139;0
WireConnection;141;1;140;0
WireConnection;131;0;162;0
WireConnection;132;0;131;3
WireConnection;132;1;131;4
WireConnection;138;0;87;0
WireConnection;138;1;127;0
WireConnection;138;2;137;0
WireConnection;143;0;138;0
WireConnection;151;0;25;0
WireConnection;148;0;156;0
WireConnection;148;1;155;0
WireConnection;145;0;147;0
WireConnection;145;1;155;0
WireConnection;147;0;101;1
WireConnection;147;1;148;0
WireConnection;157;22;126;0
WireConnection;157;17;81;0
WireConnection;157;18;35;0
WireConnection;157;19;86;0
WireConnection;157;20;153;0
WireConnection;157;21;154;0
WireConnection;158;0;99;1
WireConnection;158;1;145;0
WireConnection;152;0;26;0
WireConnection;42;0;93;0
WireConnection;42;2;43;0
WireConnection;101;1;144;0
WireConnection;164;0;165;0
WireConnection;164;1;122;0
WireConnection;127;16;132;0
WireConnection;127;20;133;0
WireConnection;90;0;89;1
WireConnection;90;1;89;2
WireConnection;168;0;167;1
WireConnection;168;1;167;3
WireConnection;169;1;90;0
WireConnection;169;0;168;0
WireConnection;170;0;169;0
WireConnection;178;0;186;0
WireConnection;179;0;178;0
WireConnection;179;1;180;0
WireConnection;179;2;181;0
WireConnection;184;0;121;0
WireConnection;184;1;183;0
WireConnection;175;1;176;0
WireConnection;175;0;179;0
WireConnection;182;0;175;0
WireConnection;166;0;164;0
WireConnection;177;0;174;0
WireConnection;186;0;185;0
WireConnection;186;1;177;0
ASEEND*/
//CHKSM=015BBD362375EAC332203CB621A21348A866CE5B