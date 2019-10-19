#version 330

out vec4 outputColor;

in vec2 texCoord;

uniform sampler2D texture0;
uniform sampler2D texture1;

void main()
{
	vec4 c1 = texture(texture0, texCoord) * texCoord.x;
	vec4 c2 = texture(texture1, texCoord) * (1 - texCoord.x);
    outputColor = c1 + c2;
}