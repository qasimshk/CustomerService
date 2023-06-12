namespace customer.data.Entities;

using System.ComponentModel.DataAnnotations;

public abstract class Entity
{
    private int? _requestedHashCode;
    private int _id;

    [Key]
    public virtual int Id
    {
        get => _id;
        protected set => _id = value;
    }

    public bool IsTransient() => Id == default;

    public override bool Equals(object obj)
    {
        if (obj is null or not Entity)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (GetType() != obj.GetType())
        {
            return false;
        }

        var item = (Entity)obj;

        if (item.IsTransient() || IsTransient())
        {
            return false;
        }

        return item.Id == Id;
    }

    public override int GetHashCode()
    {
        if (!IsTransient())
        {
            if (!_requestedHashCode.HasValue)
            {
                // XOR for random distribution (http://blogs.msdn.com/b/ericlippert/archive/2011/02/28/guidelines-and-rules-for-gethashcode.aspx)
                _requestedHashCode = Id.GetHashCode() ^ 31;
            }

            return _requestedHashCode.Value;
        }

        return base.GetHashCode();
    }

    public static bool operator ==(Entity left, Entity right)
    {
        if (Equals(left, null))
        {
            return Equals(right, null);
        }

        return left.Equals(right);
    }

    public static bool operator !=(Entity left, Entity right) => !(left == right);
}
