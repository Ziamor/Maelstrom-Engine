#version 330

out vec4 outputColor;

in vec2 texCoord;
in vec3 normal; 
in vec3 worldPos;

uniform sampler2D texture0;
uniform vec4 lightColor;
uniform vec3 lightPos;

void main()
{
	vec3 lightDir = normalize(lightPos - worldPos);
	float nDotL = max(dot(normal, lightDir), 0.0);

	float ambientStrength = 0.1;
	vec4 ambient = ambientStrength * lightColor;

	vec4 diffuse = texture(texture0, texCoord);

    outputColor = diffuse * (nDotL + ambient);
}