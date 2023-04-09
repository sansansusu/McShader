#version 120

attribute vec4 mc_Entity;

varying float entityType;
varying vec4 color;
varying vec4 texcoord;
varying vec2 normal;

vec2 normalEncode(vec3 n) {
    vec2 enc = normalize(n.xy) * (sqrt(-n.z*0.5+0.5));
    enc = enc*0.5+0.5;
    return enc;
}

void main() {
	vec4 position = gl_ModelViewMatrix * gl_Vertex;
	gl_Position = gl_ProjectionMatrix * position;
	gl_FogFragCoord = length(position.xyz);
  //云都是在gbuffers_textured着色器中渲染的,然而除了云以外还有其他东西也使用gbuffers_textured,因此判断云的关键办法是通过顶点属性mc_Entity,之前我们在制作草木摆动效果时就用到了mc_Entity,文档只说了在渲染地形时mc_Entity.x用于表示砖块类型,实际上在gbuffers_textured中当mc_Entity.x等于-3.0时代表正在渲染云,只要在顶点着色器中将mc_Entity.x提取出来并传入片元着色器,然后在片元着色器中判断它是否<-2.5(如果是!=-3.0可能会有问题) 如果是的话就discard掉就行了.
  entityType = mc_Entity.x;
	color = gl_Color;
	texcoord = gl_TextureMatrix[0] * gl_MultiTexCoord0;
	normal = normalEncode(gl_NormalMatrix * gl_Normal);
}
