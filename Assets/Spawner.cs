using UnityEngine;

public class Spawner : MonoBehaviour
{
	public GameObject prefab;
	public int spawnCount;
	public bool propertyBlock;
	public bool randomColor;
	public int randomColorCount;
	public bool randomTextures;
	public int randomTexturesCount;
	private Color[] _colors;
	private Material[] _materials;
	private MaterialPropertyBlock[] _blocks;
	private Texture2D[] _textures;

	// Start is called before the first frame update
	private void Start()
	{
		if ( randomColor )
		{
			_colors = new Color[randomColorCount];
			for ( int i = 0; i < randomColorCount; i++ )
			{
				var color = Random.ColorHSV();
				color.a = 1;
				_colors[i] = color;
			}
		}
		else
		{
			_colors = new Color[1];
			_colors[0] = new Color( 1, 1, 0 );
		}
		if ( randomTextures )
		{
			_textures = new Texture2D[randomTexturesCount];
			for ( int i = 0; i < randomTexturesCount; i++ )
			{
				_textures[i] = new Texture2D( 1024, 1024, TextureFormat.RGB24, true, true );
				var pixels = _textures[i].GetPixels();
				for ( int x = 0; x < 1024; x++ )
				{
					for ( int y = 0; y < 1024; y++ )
					{
						pixels[y + x * 1024] = Random.ColorHSV();
					}
				}
				_textures[i].SetPixels( pixels );
				_textures[i].Apply();
			}
		}
		else
		{
			_textures = new Texture2D[1];
			_textures[0] = new Texture2D( 1024, 1024, TextureFormat.RGB24, true, true );
			var pixels = _textures[0].GetPixels();
			for ( int x = 0; x < 1024; x++ )
			{
				for ( int y = 0; y < 1024; y++ )
				{
					pixels[y + x * 1024] = Random.ColorHSV();
				}
			}
			_textures[0].SetPixels( pixels );
			_textures[0].Apply();
		}

		for ( int i = 0; i < spawnCount; i++ )
		{
			var instance = Instantiate( prefab, Random.insideUnitSphere * 30, Quaternion.identity );
			if ( propertyBlock )
			{
				SetupByProperty( instance );
			}
			else
			{
				SetupByMaterial( instance );
			}
		}
	}

	private void SetupByProperty( GameObject instance )
	{
		var renderer = instance.GetComponent<Renderer>();

		if ( _blocks == null )
		{
			_blocks = new MaterialPropertyBlock[_colors.Length * _textures.Length];
			for ( var i = 0; i < _colors.Length; i++ )
			{
				var color = _colors[i];
				for ( int j = 0; j < _textures.Length; j++ )
				{
					_blocks[j + i * _textures.Length] = new MaterialPropertyBlock();
					_blocks[j + i * _textures.Length].SetColor( "_Color", color );
					_blocks[j + i * _textures.Length].SetTexture( "_MainTex", _textures[j] );
				}
			}
		}

		renderer.SetPropertyBlock( _blocks[Random.Range( 0, _blocks.Length )] );
	}

	private void SetupByMaterial( GameObject instance )
	{
		var renderer = instance.GetComponent<Renderer>();

		if ( _materials == null )
		{
			_materials = new Material[_colors.Length * _textures.Length];
			for ( var i = 0; i < _colors.Length; i++ )
			{
				var color = _colors[i];
				for ( int j = 0; j < _textures.Length; j++ )
				{
					_materials[j + i * _textures.Length] = new Material( renderer.sharedMaterial )
					{
						color = color,
						mainTexture = _textures[j],
					};
				}
			}
		}

		renderer.sharedMaterial = _materials[Random.Range( 0, _materials.Length )];
	}
}