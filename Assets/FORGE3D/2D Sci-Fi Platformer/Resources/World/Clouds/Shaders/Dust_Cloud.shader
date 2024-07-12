// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Dust_Cloud"
{
	Properties
	{
		_MainTex("_MainTex", 2D) = "white" {}
		_UVDistortion("UV Distortion", 2D) = "white" {}
		_Tint("Tint", Color) = (1,1,1,1)
		_UVPan("UV Pan", Vector) = (0.1,0,0,0)
		_UVDist("UV Dist", Range( 0 , 1)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit alpha:fade keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
			float4 vertexColor : COLOR;
		};

		uniform sampler2D _MainTex;
		uniform sampler2D _UVDistortion;
		uniform float4 _UVDistortion_ST;
		uniform float2 _UVPan;
		uniform float _UVDist;
		uniform float4 _Tint;

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_UVDistortion = i.uv_texcoord * _UVDistortion_ST.xy + _UVDistortion_ST.zw;
			float2 panner15 = ( _Time.y * _UVPan + i.uv_texcoord);
			float4 tex2DNode2 = tex2D( _MainTex, ( uv_UVDistortion + ( tex2D( _UVDistortion, panner15 ).r * _UVDist ) ) );
			o.Emission = ( tex2DNode2.r * i.vertexColor * _Tint ).rgb;
			float4 tex2DNode28 = tex2D( _UVDistortion, uv_UVDistortion );
			o.Alpha = ( tex2DNode2.a * i.vertexColor.a * tex2DNode28.r * _Tint.a * tex2DNode28.a );
		}

		ENDCG
	}
}
/*ASEBEGIN
Version=18921
-1787;136;1589;922;1303.934;886.939;1.681338;True;True
Node;AmplifyShaderEditor.Vector2Node;18;-1560.457,-93.56689;Float;False;Property;_UVPan;UV Pan;3;0;Create;True;0;0;0;False;0;False;0.1,0;0.06,-0.01;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;13;-1412,-240.5;Inherit;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TimeNode;29;-1307.227,106.3395;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TexturePropertyNode;27;-1307.993,-584.8533;Float;True;Property;_UVDistortion;UV Distortion;1;0;Create;True;0;0;0;False;0;False;None;92e8a767d9370ad4b9137a6023eae758;False;white;Auto;Texture2D;-1;0;2;SAMPLER2D;0;SAMPLERSTATE;1
Node;AmplifyShaderEditor.PannerNode;15;-1104,-237.5;Inherit;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.1,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;21;-831.2462,-44.5761;Float;False;Property;_UVDist;UV Dist;4;0;Create;True;0;0;0;False;0;False;0;0.447;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;3;-905,-261.5;Inherit;True;Property;_UVDistortionMask;UV Distortion Mask;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;-538.5884,-282.4042;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;19;-833.4239,-408.1177;Inherit;False;0;2;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;24;-348.789,-402.004;Inherit;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.ColorNode;32;170.5892,-432.1772;Float;False;Property;_Tint;Tint;2;0;Create;True;0;0;0;False;0;False;1,1,1,1;1,0.8926978,0.6617647,0.566;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.VertexColorNode;4;103.5735,-14.46266;Inherit;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;28;-977.6147,-692.9629;Inherit;True;Property;_TextureSample0;Texture Sample 0;1;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;69.87358,-240.5627;Inherit;True;Property;_MainTex;_MainTex;0;0;Create;True;0;0;0;False;0;False;-1;None;2ea06842d0c68594c9ee63e80e1a4d4e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;5;418.6735,-102.8626;Inherit;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;22;-253.091,-551.8382;Inherit;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SinTimeNode;30;-1561.227,58.33948;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;434.6735,55.13734;Inherit;False;5;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;4;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;16;-1329,-74.5;Inherit;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;38;652.6736,-150.3627;Float;False;True;-1;2;;0;0;Unlit;Dust_Cloud;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;All;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;15;0;13;0
WireConnection;15;2;18;0
WireConnection;15;1;29;2
WireConnection;3;0;27;0
WireConnection;3;1;15;0
WireConnection;25;0;3;1
WireConnection;25;1;21;0
WireConnection;19;2;27;0
WireConnection;24;0;19;0
WireConnection;24;1;25;0
WireConnection;28;0;27;0
WireConnection;2;1;24;0
WireConnection;5;0;2;1
WireConnection;5;1;4;0
WireConnection;5;2;32;0
WireConnection;9;0;2;4
WireConnection;9;1;4;4
WireConnection;9;2;28;1
WireConnection;9;3;32;4
WireConnection;9;4;28;4
WireConnection;38;2;5;0
WireConnection;38;9;9;0
ASEEND*/
//CHKSM=D6C04648C0A87618C82B6DF36558707ABE320283