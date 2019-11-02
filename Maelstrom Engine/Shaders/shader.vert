#version 330 core

layout(location = 0) in vec3 aPosition;
layout(location = 1) in vec3 aNormal;
layout(location = 2) in vec3 aTangent;;
layout(location = 3) in vec2 aTexCoord;

out vec2 texCoord;
out vec3 normal;
out vec3 worldPos;
out mat3 TBN;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;
uniform mat4 normalMat;

void main(void)
{
    gl_Position = vec4(aPosition, 1.0) * model * view * projection;
	worldPos = vec3(vec4(aPosition, 1.0) * model);
	texCoord = aTexCoord;
	normal = normalize(aNormal * mat3(normalMat));

	vec3 T = normalize(vec3(vec4(aTangent, 0.0) * model));
	vec3 B = normalize(vec3(vec4(cross(aNormal, aTangent), 0.0) * model));
	vec3 N = normalize(vec3(vec4(aNormal, 0.0) * model));
	TBN = transpose(mat3(T, B, N));
}