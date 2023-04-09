#version 120

uniform int fogMode;
uniform sampler2D texture;

varying float entityType;
varying vec4 color;
varying vec4 texcoord;
varying vec2 normal;

/* DRAWBUFFERS:02 */
void main() {
	if(entityType < -2.5)
    discard;
	gl_FragData[0] = texture2D(texture, texcoord.st) * color;
	if(fogMode == 9729)
		gl_FragData[0].rgb = mix(gl_Fog.color.rgb, gl_FragData[0].rgb, clamp((gl_Fog.end - gl_FogFragCoord) / (gl_Fog.end - gl_Fog.start), 0.0, 1.0));
	else if(fogMode == 2048)
		gl_FragData[0].rgb = mix(gl_Fog.color.rgb, gl_FragData[0].rgb, clamp(exp(-gl_FogFragCoord * gl_Fog.density), 0.0, 1.0));
	gl_FragData[1] = vec4(normal, 0.0, 1.0);
}
