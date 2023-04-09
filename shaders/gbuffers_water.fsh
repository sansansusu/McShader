#version 120

//gbuffers_water除了渲染水之外还要渲染其它半透明的物体

//根据视点到镜面像素的射线,和镜面的法线,计算出反射光线.
//以像素对应的位置为起点,以反射光线为方向,进行Ray Tracing.
//在Ray Tracing中找到第一个命中的像素.
//取此像素对应的颜色作为反射颜色,如没有命中则返回一个预先指定的颜色.
//眼与水平面法线形成的的对角线是否命中实体（不知道对不对）

uniform int fogMode;
uniform sampler2D texture;
uniform sampler2D lightmap;

varying vec4 color;
varying vec4 texcoord;
varying vec4 lmcoord;
varying vec2 normal;
varying float attr;
/* DRAWBUFFERS:024 */

void main() {
    gl_FragData[0] = texture2D(texture, texcoord.st) * texture2D(lightmap, lmcoord.st) * color;
    if(fogMode == 9729)
        gl_FragData[0].rgb = mix(gl_Fog.color.rgb, gl_FragData[0].rgb, clamp((gl_Fog.end - gl_FogFragCoord) / (gl_Fog.end - gl_Fog.start), 0.0, 1.0));
    else if(fogMode == 2048)
        gl_FragData[0].rgb = mix(gl_Fog.color.rgb, gl_FragData[0].rgb, clamp(exp(-gl_FogFragCoord * gl_Fog.density), 0.0, 1.0));
    gl_FragData[1] = vec4(normal, 0.0, 1.0);
    gl_FragData[2] = vec4(attr, 0.0, 0.0, 1.0);
}
