Shader "Custom/FresnelReflection" {

Properties {

_MainTint("Diffuse Tint",Color)=(1,1,1,1)

_MainTex ("Base (RGB)", 2D) = "white" {}

_CubeMap("CubeMap",CUBE)=""{}

_ReflAmount("Reflection Amount",Range(0.1,3))=0.5

_RimPower("Fresnel Falloff",Range(0.1,3))=1

//反射颜色;

_SpecColor("Specular Color",Color)=(1,1,1,1)

//反射强度值;

_SpecPower("Specular Power",Range(0.02,1))=0.5

}

SubShader {

Tags { "RenderType"="Opaque" }

LOD 200

 

CGPROGRAM

//使用unity内置的BlinnPhong光照模型;
//#pragma surface surfaceFunction lightModel[optionalparams]
// surfaceFunction：surfaceShader函数，形如void surf (Input IN, inoutSurfaceOutput o)
// lightModel：使用的光照模式。包括Lambert（漫反射）和BlinnPhong（镜面反射）。
//  也可以自己定义光照函数。比如编译指令为#pragma surface surf MyCalc
//   在Shader里定义half4 LightingMyCalc (SurfaceOutputs, 参数略)函数进行处理(函数名在签名加上了“Lighting”）。
// 定义输入数据结构（比如上面的Input）、编写自己的Surface函数处理输入、最终输出修改过后的SurfaceOutput。而SurfaceOutput的定义为:
//struct SurfaceOutput  
//{  
//    half3 Albedo; // 纹理颜色值（r, g, b)  
//    half3 Normal; // 法向量(x, y, z)  
//    half3 Emission; // 自发光颜色值(r, g, b)  
//    half Specular; // 镜面反射度  
//    half Gloss; // 光泽度  
//    half Alpha; // Alpha不透明度  
//};  

//输入结构    
        //struct Input     
        //{    
        //    float2 uv_MainTex;//纹理贴图    
        //    float2 uv_BumpMap;//法线贴图    
        //    float3 viewDir;//观察方向    
        //}; 

#pragma surface surf BlinnPhong

//使用Shader Mode 3.0;

#pragma target 3.0

 

sampler2D _MainTex;

samplerCUBE _CubeMap;

float4 _MainTint;

float _ReflAmount;

float _RimPower;

float _SpecPower;

 

struct Input {

float2 uv_MainTex;

float3 worldRefl;

float3 viewDir;

};

 

void surf (Input IN, inout SurfaceOutput o) {

half4 c = tex2D (_MainTex, IN.uv_MainTex);

 //反转saturate函数获得的0-1之间的值;

float rim = 1.0 - saturate(dot(o.Normal,normalize(IN.viewDir)));

rim = pow(rim,_RimPower);

 //计算反射率;

o.Albedo = c.rgb*_MainTint.rgb;

//计算自发光并赋值;
//环境映射贴图+Rim
o.Emission = (texCUBE(_CubeMap,IN.worldRefl).rgb*_ReflAmount)*rim;

//镜面赋值;
//高光
o.Specular = _SpecPower;

//光泽度赋值;
o.Gloss = 1;

o.Alpha = c.a;

}

ENDCG

} 

FallBack "Diffuse"

}