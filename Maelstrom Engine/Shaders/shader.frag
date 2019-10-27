#version 330

out vec4 outputColor;

in vec2 texCoord;
in vec3 normal; 
in vec3 worldPos;
in mat3 TBN;
in vec3 test;

uniform sampler2D diffuseTex;
uniform sampler2D normalMapTex;

uniform vec4 lightColor;
uniform vec3 lightPos;

void main()
{
	vec3	normalMap = texture(normalMapTex, texCoord).rgb;
			normalMap = normalize(normalMap * 2.0 - 1.0);   
			normalMap = normalize(TBN * normalMap); 

	vec3 lightDir = normalize(lightPos - worldPos);
	float nDotL = max(dot(normalMap, lightDir), 0.0);

	float ambientStrength = 0.1;
	vec4 ambient = ambientStrength * lightColor;

	vec4 diffuse = texture(diffuseTex, texCoord);

    outputColor = diffuse * (nDotL + ambient);
}