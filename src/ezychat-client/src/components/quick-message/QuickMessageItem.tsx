import React from "react";
import { Button } from "@/components/ui/button";
import { Badge } from "@/components/ui/badge";
import { Copy, Edit2, Trash2 } from "lucide-react";
import { QuickMessage } from "@/types";

interface QuickMessageItemProps {
  message: QuickMessage;
  onUse?: (content: string) => void;
  onEdit: (message: QuickMessage) => void;
  onDelete: (id: string) => void;
}

export const QuickMessageItem: React.FC<QuickMessageItemProps> = ({
  message,
  onUse,
  onEdit,
  onDelete,
}) => {
  return (
    <div className="border rounded-lg p-3 hover:bg-muted/50 transition-colors">
      <div className="flex items-start justify-between gap-2">
        <div className="flex-1 min-w-0">
          <div className="flex items-center gap-2 mb-2">
            <Badge variant="secondary" className="font-mono">
              {message.key}
            </Badge>
          </div>
          <p className="text-sm text-muted-foreground break-words max-h-[50px] overflow-hidden line-clamp-4">
            {message.content}
          </p>
        </div>
        <div className="flex gap-1">
          {onUse && (
            <Button
              variant="ghost"
              size="icon"
              onClick={() => onUse(message.content)}
              title="Use this message"
            >
              <Copy className="w-4 h-4" />
            </Button>
          )}
          <Button
            variant="ghost"
            size="icon"
            onClick={() => onEdit(message)}
            title="Edit"
          >
            <Edit2 className="w-4 h-4" />
          </Button>
          <Button
            variant="ghost"
            size="icon"
            onClick={() => onDelete(message.id)}
            title="Delete"
          >
            <Trash2 className="w-4 h-4 text-destructive" />
          </Button>
        </div>
      </div>
    </div>
  );
};
