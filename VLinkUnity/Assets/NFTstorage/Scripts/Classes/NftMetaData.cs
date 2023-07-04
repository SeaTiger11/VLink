using System;
using System.Collections.Generic;
using System.ComponentModel;
using Newtonsoft.Json;

namespace NFTstorage.ERC721
{
    [Serializable]
    public class NftMetaData
    {
        [DefaultValue("")]
        public string title;

        // A human readable description of the item
        [DefaultValue("")]
        public string type;

        // These are the attributes for the item, which will
        // show up on the OpenSea page for the item
        [DefaultValue(null)]
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<Property> properties;

        public NftMetaData(string _name)
        {
            Property name = new Property();
            name.type = "string";
            name.description = _name;
            properties.Add(name);
        }

        public Property image = new Property();

        /// <summary>
        /// Set IPFS path by passing CID
        /// </summary>
        /// <param name="cid"></param>
        public void SetIPFS(string cid)
        {
            image.type = "string";
            image.description = "ipfs://" + cid;
            properties.Add(image);
        }
    }

    [Serializable]
    public class Property
    {
        // optional. the name of the trait.
        [DefaultValue("")]
        public string type;

        // the value of the trait. Not optional.
        public object description;
    }
}