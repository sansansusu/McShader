#version 120

#define SHADOW_MAP_BIAS 0.85  //一班用系数为0.85的语言投影越接近1浸出的占比越大
#define CLOUD_MIN 400.0 //云朵最小高度
#define CLOUD_MAX 4300.0 //云朵最大高度


const int RG16 = 0;
const int RGB8 = 0;
const int colortex1Format = RGB8;
const int gnormalFormat = RG16;
const int shadowMapResolution = 2048;  // 阴影分辨率 默认 1024
const float sunPathRotation = -25.0;  // 太阳偏移角 默认 0
const bool shadowHardwareFiltering = true;

//shadermod中提供了8个颜色缓冲区，颜色缓冲区用于在不同着色器中传递数据
//其中shadermod中的缓冲区只能在composite和final中可用
//0号缓冲的颜色最终被输出
//uniform是外部传递给shader的变量类型是sampler2D。
//texture用来表示纹理颜色
//texcoord是用来表示表示在整张纹理中的位置
//depthtex0也是一张纹理不过是深度纹理  用来存的是一张深度值图片
//shadow纹理也是深度纹理，只不过是在太阳视角下的深度用来绘制阴影用的。

uniform float far;
uniform vec3 sunPosition;
//用来计算观察方向
uniform vec3 cameraPosition;  //玩家在世界坐标系中的位置,可以用作表示视点中心，
uniform mat4 gbufferProjectionInverse;
uniform mat4 gbufferModelViewInverse;
uniform mat4 shadowModelView;
uniform mat4 shadowProjection;
uniform sampler2D gcolor; //颜色纹理
uniform sampler2D gnormal;  //
uniform sampler2D depthtex0;  //深度纹理
uniform sampler2D noisetex; //噪声纹理
uniform sampler2DShadow shadow; //太阳视角下深度纹理

uniform float frameTimeCounter;

varying vec3 worldSunPosition;  //顶点着色器传来的世界坐标系下的光源向量
varying float extShadow;
varying vec3 lightPosition;
varying vec4 texcoord;
varying vec3 cloudBase1;
varying vec3 cloudBase2;
varying vec3 cloudLight1;
varying vec3 cloudLight2;


/*光照函数
@sum 累积颜色
@density密度
@diff 两点间密度的差值
color就是由密度控制的
lighting则是由密度间接推出的光照
*/
vec4 cloudLighting(vec4 sum, float density, float diff) {
  vec4 color = vec4(mix(cloudBase1, cloudBase2, density ), density );
  vec3 lighting = mix(cloudLight1, cloudLight2, diff);
    color.xyz *= lighting;
    color.a *= 0.4;
    color.rgb *= color.a;
    return sum + color*(1.0-sum.a);
}

//noise函数为噪音函数
float noise(vec3 x)
{
    vec3 p = floor(x);  //向下取整
    vec3 f = fract(x);  //fract为取小数部分
    f = smoothstep(0.0, 1.0, f);

    vec2 uv = (p.xy+vec2(37.0, 17.0)*p.z) + f.xy;
    float v1 = texture2D( noisetex, (uv)/256.0, -100.0 ).x;
    float v2 = texture2D( noisetex, (uv + vec2(37.0, 17.0))/256.0, -100.0 ).x;
    return mix(v1, v2, f.z);
}

float getCloudNoise(vec3 worldPos) {
    vec3 coord = worldPos;
    float v = 1.0;
    if(coord.y < CLOUD_MIN)
    {
      v = 1.0 - smoothstep(0.0, 1.0, min(CLOUD_MIN - coord.y, 1.0));
    }
    else if(coord.y > CLOUD_MAX)
    {
      v = 1.0 - smoothstep(0.0, 1.0, min(coord.y - CLOUD_MAX, 1.0));
    }
    coord.x += frameTimeCounter * 5.0;
    coord *= 0.002;
    float n  = noise(coord) * 0.5;   coord *= 3.0;
          n += noise(coord) * 0.25;  coord *= 3.01;
          n += noise(coord) * 0.125; coord *= 3.02;
          n += noise(coord) * 0.0625;
    n *= v;
    return smoothstep(0.0, 1.0, pow(max(n - 0.5, 0.0) * (1.0 / (1.0 - 0.5)), 0.5));
}

/*
  @startPoint 世界坐标系下的视点中心
  @direction  观察方向
  @bgColor  原像素颜色
  @maxDis 未经规整化的最大距离
  (1单位长度等于游戏中的1格)
*/
vec3 cloudRayMarching(vec3 startPoint, vec3 direction, vec3 bgColor, float maxDis) {
    if(direction.y <= 0.1)
        return bgColor;
    vec3 testPoint = startPoint;
    float cloudMin = startPoint.y + CLOUD_MIN * (exp(-startPoint.y / CLOUD_MIN) + 0.001);
    float d = (cloudMin - startPoint.y) / direction.y;
    testPoint += direction * (d + 0.01);
    if(distance(testPoint, startPoint) > maxDis)
        return bgColor;
    float cloudMax = cloudMin + (CLOUD_MAX - CLOUD_MIN);
    direction *= 1.0 / direction.y;
    vec4 final = vec4(0.0);
    float fadeout = (1.0 - clamp(length(testPoint) / (far * 100.0) * 6.0, 0.0, 1.0));
    for(int i = 0; i < 32; i++)
    {
        if(final.a > 0.99 || testPoint.y > cloudMax)
            continue;
        testPoint += direction;
        vec3 samplePoint = vec3(testPoint.x, testPoint.y - cloudMin + CLOUD_MIN, testPoint.z);
        float density = getCloudNoise(samplePoint) * fadeout;
        if(density > 0.0)
        {
            float diff = clamp((density - getCloudNoise(samplePoint + worldSunPosition * 10.0)) * 10.0, 0.0, 1.0 );
            final = cloudLighting(final, density, diff);
        }
    }
    final = clamp(final, 0.0, 1.0);
    return bgColor * (1.0 - final.a) + final.rgb;
}

vec3 normalDecode(vec2 enc) {
    vec4 nn = vec4(2.0 * enc - 1.0, 1.0, -1.0);
    float l = dot(nn.xyz,-nn.xyw);
    nn.z = l;
    nn.xy *= sqrt(l);
    return nn.xyz * 2.0 + vec3(0.0, 0.0, -1.0);
}
float shadowMapping(vec4 worldPosition, float dist, vec3 normal, float alpha) {
  if(dist > 0.9)
		return extShadow;
	float shade = 0.0;
  //光照方向与法线方向点成可以判断两个方向是否接近范围是[-1,1]越接近1方向越相似
	float angle = dot(lightPosition, normal);
  //如果角度太小就直接涂黑
	if(angle <= 0.1 && alpha > 0.99)
	{
		shade = 1.0;
	}
	else
	{
		vec4 shadowposition = shadowModelView * worldPosition;
		shadowposition = shadowProjection * shadowposition;
		float edgeX = abs(shadowposition.x) - 0.9;
		float edgeY = abs(shadowposition.y) - 0.9;
		float distb = sqrt(shadowposition.x * shadowposition.x + shadowposition.y * shadowposition.y);
		float distortFactor = (1.0 - SHADOW_MAP_BIAS) + distb * SHADOW_MAP_BIAS;
		shadowposition.xy /= distortFactor;
		shadowposition /= shadowposition.w;
		shadowposition = shadowposition * 0.5 + 0.5;
		shade = 1.0 - shadow2D(shadow, vec3(shadowposition.st, shadowposition.z - 0.0001)).z;
    //如果角度略小的话,就将它过渡到全黑
    //alpha是传过来的color.a的值如果alpha小于1的才涂黑因为云和水不能涂黑
    if(angle < 0.2 && alpha > 0.99)
			shade = max(shade, 1.0 - (angle - 0.1) * 10.0);
		shade -= max(0.0, edgeX * 10.0);
		shade -= max(0.0, edgeY * 10.0);
	}
  shade -= clamp((dist - 0.7) * 5.0, 0.0, 1.0);  //在l处于0.7~0.9的地方进行渐变过渡
  shade = clamp(shade, 0.0, 1.0);  //避免出现过大或过小
	return max(shade, extShadow);
}



void main() {
	vec4 color = texture2D(gcolor, texcoord.st); //颜色
	vec3 normal = normalDecode(texture2D(gnormal, texcoord.st).rg);  //法线
	float depth = texture2D(depthtex0, texcoord.st).x; //深度
  //利用深度缓冲建立带深度的ndc坐标
  //深度缓冲rgb的r通道表示该像素在屏幕空间中的深度值范围是[0,1]
  //所以要乘以2减1作为ndc坐标的Z轴（正负）
  vec4 viewPosition = gbufferProjectionInverse * vec4(texcoord.s * 2.0 - 1.0, texcoord.t * 2.0 - 1.0, 2.0 * depth - 1.0, 1.0f);
	viewPosition /= viewPosition.w;
	vec4 worldPosition = gbufferModelViewInverse * (viewPosition + vec4(normal * 0.05 * sqrt(abs(viewPosition.z)), 0.0));
	float dist = length(worldPosition.xyz) / far;

	float shade = shadowMapping(worldPosition, dist, normal, color.a);
	color.rgb *= 1.0 - shade * 0.5;

  vec3 rayDir = normalize(gbufferModelViewInverse * viewPosition).xyz;
  if(dist > 0.9999)
    dist = 100.0;
  color.rgb = cloudRayMarching(cameraPosition, rayDir, color.rgb, dist * far);


	float brightness = dot(color.rgb, vec3(0.2126, 0.7152, 0.0722));
	vec3 highlight = color.rgb * max(brightness - 0.25, 0.0);

/* DRAWBUFFERS:01 */
	gl_FragData[0] = color;
	gl_FragData[1] = vec4(highlight, 1.0);
}
