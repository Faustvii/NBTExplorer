﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Substrate
{
    using NBT;
    using Utility;

    public class Block : IBlock, ICopyable<Block>
    {
        private int _id;
        private int _data;
        private int _skylight;
        private int _blocklight;

        private TileEntity _tileEntity;

        public BlockInfo Info
        {
            get { return BlockInfo.BlockTable[_id]; }
        }

        public int ID
        {
            get { return _id; }
            set
            {
                BlockInfoEx info1 = BlockInfo.BlockTable[_id] as BlockInfoEx;
                BlockInfoEx info2 = BlockInfo.BlockTable[value] as BlockInfoEx;

                if (info1 != info2) {
                    if (info1 != null) {
                        _tileEntity = null;
                    }

                    if (info2 != null) {
                        _tileEntity = TileEntityFactory.Create(info2.TileEntityName);
                    }
                }

                _id = value;
            }
        }

        public int Data
        {
            get { return _data; }
            set
            {
                if (BlockManager.EnforceDataLimits && BlockInfo.BlockTable[_id] != null) {
                    if (!BlockInfo.BlockTable[_id].TestData(value)) {
                        return;
                    }
                }
                _data = value;
            }
        }

        public int SkyLight
        {
            get { return _skylight; }
            set { _skylight = value; }
        }

        public int BlockLight
        {
            get { return _blocklight; }
            set { _blocklight = value; }
        }

        public Block (int id)
        {
            _id = id;
        }

        public Block (int id, int data)
        {
            _id = id;
            _data = data;
        }

        public Block (IChunk chunk, int lx, int ly, int lz)
        {
            _id = chunk.GetBlockID(lx, ly, lz);
            _data = chunk.GetBlockData(lx, ly, lz);
            _skylight = chunk.GetBlockSkyLight(lx, ly, lz);
            _blocklight = chunk.GetBlockLight(lx, ly, lz);
            _tileEntity = chunk.GetTileEntity(lx, ly, lz).Copy();
        }

        public TileEntity GetTileEntity ()
        {
            return _tileEntity;
        }

        public bool SetTileEntity (TileEntity te)
        {
            BlockInfoEx info = BlockInfo.BlockTable[_id] as BlockInfoEx;
            if (info == null) {
                return false;
            }

            if (te.GetType() != TileEntityFactory.Lookup(info.TileEntityName)) {
                return false;
            }

            _tileEntity = te;
            return true;
        }

        public bool ClearTileEntity ()
        {
            _tileEntity = null;
            return true;
        }

        #region ICopyable<Block> Members

        public Block Copy ()
        {
            Block block = new Block(_id, _data);
            block._blocklight = _blocklight;
            block._skylight = _skylight;
            block._tileEntity = _tileEntity.Copy();

            return block;
        }

        #endregion
    }
}