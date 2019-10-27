#version 330

out vec4 outputColor;

in vec2 texCoord;
in vec3 normal; 
in vec3 worldPos;

uniform sampler2D texture0;
uniform vec4 lightColor;

void main()
{
	vec3 lightPos = vec3(0,0,0);

	float ambientStrength = 0.01;
	vec4 ambient = ambientStrength * lightColor;

	vec3 lightDir = normalize(lightPos - worldPos);

	float nDotL = max(dot(normal, lightDir), 0.0);

    outputColor = texture(texture0, texCoord) * nDotL + ambient;
}