#version 330

out vec4 outputColor;

in vec2 texCoord;
in vec3 normal; 
in vec3 worldPos;
in mat3 TBN;

uniform sampler2D diffuseTex;
uniform sampler2D normalMapTex;
uniform sampler2D specularMapTex;

uniform vec4 lightColor;
uniform vec3 viewPos;
uniform vec3 lightPos;

void main()
{
	vec3	normalMap = texture(normalMapTex, texCoord).rgb;
			normalMap = normalize(normalMap * 2.0 - 1.0);   
			normalMap = normalize(TBN * normalMap); 

	vec3 lightDir = normalize(lightPos - worldPos);
	float nDotL = max(dot(normal, lightDir), 0.0);

	float ambientStrength = 0.1;
	vec4 ambient = ambientStrength * lightColor;

	vec4 diffuse = texture(diffuseTex, texCoord);	

	float specularStrength = 0.5;

	vec3 viewDir = normalize(viewPos - worldPos);
	vec3 reflectDir = reflect(-lightDir, normal);
	float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);

	vec4 specularMap = texture(specularMapTex, texCoord);
	vec4 specular = specularStrength * spec * lightColor * specularMap; 

    outputColor = diffuse * (nDotL + ambient) + specular;
}