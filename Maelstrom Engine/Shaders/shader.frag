#version 330

out vec4 outputColor;

in vec2 texCoord;
in vec3 normal; 
in vec3 worldPos;
in mat3 TBN;

uniform sampler2D diffuseTex;
uniform sampler2D normalMapTex;
uniform sampler2D specularMapTex;

struct DirectionalLight {
	vec3 dir;
	vec3 diffuse;

	float ambientStrength;
};

struct PointLight {
	vec3 position;
	vec3 diffuse;

	float constant;
	float linear;
	float quadratic;
};

struct Material {
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
    float shininess;
};   

uniform DirectionalLight directionalLight;
uniform Material material;

#define NR_POINT_LIGHTS 2  
uniform PointLight pointLights[NR_POINT_LIGHTS];

uniform vec3 viewPos;

vec3 CalcDirectionalLight(DirectionalLight light, vec3 normal, vec3 viewDir) {
	vec3 lightDir = normalize(-light.dir);
	float nDotL = max(dot(normal, lightDir), 0.0);

	float specularStrength = 0.5;

	vec3 reflectDir = reflect(-lightDir, normal);
	float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);

	vec3 specularMap = texture(specularMapTex, texCoord).xyz;
	vec3 specular = specularStrength * spec * light.diffuse * specularMap; 	

	vec3 ambient = light.ambientStrength * light.diffuse * texture(diffuseTex, texCoord).xyz;
	vec3 diffuse = light.diffuse * nDotL * texture(diffuseTex, texCoord).xyz;

	return diffuse + ambient + specular;
}

vec3 CalcPointLight(PointLight light, vec3 normal, vec3 worldPos, vec3 viewDir)
{
    vec3 lightDir = normalize(light.position - worldPos);
	float nDotL = max(dot(normal, lightDir), 0.0);

	float specularStrength = 0.5;

	vec3 reflectDir = reflect(-lightDir, normal);
	float spec = pow(max(dot(viewDir, reflectDir), 0.0), 32);

	vec3 specularMap = texture(specularMapTex, texCoord).xyz;
	vec3 specular = specularStrength * spec * light.diffuse * specularMap; 	

	//vec3 ambient = light.ambientStrength * light.diffuse * texture(diffuseTex, texCoord).xyz;
	vec3 diffuse = light.diffuse * nDotL * texture(diffuseTex, texCoord).xyz;

	float distance    = length(light.position - worldPos);
    float attenuation = 1.0 / (light.constant + light.linear * distance +  light.quadratic * (distance * distance));    

	return (diffuse + specular) * attenuation;
} 

void main()
{
	vec3	normalMap = texture(normalMapTex, texCoord).rgb;
			normalMap = normalize(normalMap * 2.0 - 1.0);   
			normalMap = normalize(TBN * normalMap); 	
	
	vec3 viewDir = normalize(viewPos - worldPos);
	
	vec3 lightValues = CalcDirectionalLight(directionalLight, normal, viewDir);

	for(int i = 0; i < NR_POINT_LIGHTS; i++)
		lightValues += CalcPointLight(pointLights[i], normal, worldPos, viewDir);
    outputColor = vec4(lightValues,1);
}