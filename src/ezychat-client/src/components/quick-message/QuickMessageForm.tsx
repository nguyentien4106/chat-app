import React from "react";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import { Textarea } from "@/components/ui/textarea";
import { X } from "lucide-react";

interface QuickMessageFormProps {
  mode: "create" | "edit";
  formData: { key: string; content: string };
  errors: { key: string; content: string };
  onFormDataChange: (data: { key: string; content: string }) => void;
  onSubmit: () => void;
  onCancel: () => void;
}

export const QuickMessageForm: React.FC<QuickMessageFormProps> = ({
  mode,
  formData,
  errors,
  onFormDataChange,
  onSubmit,
  onCancel,
}) => {
  return (
    <div className="border rounded-lg p-4 space-y-4">
      <div className="flex items-center justify-between">
        <h3 className="font-semibold">
          {mode === "edit" ? "Edit Quick Message" : "New Quick Message"}
        </h3>
        <Button variant="ghost" size="icon" onClick={onCancel}>
          <X className="w-4 h-4" />
        </Button>
      </div>

      <div className="space-y-2">
        <Label htmlFor="key">Key *</Label>
        <Input
          id="key"
          placeholder="e.g., greeting, intro, closing"
          value={formData.key}
          onChange={(e) =>
            onFormDataChange({ ...formData, key: e.target.value })
          }
          className={errors.key ? "border-red-500" : ""}
        />
        {errors.key && <p className="text-xs text-red-500">{errors.key}</p>}
      </div>

      <div className="space-y-2">
        <Label htmlFor="content">Content *</Label>
        <Textarea
          id="content"
          placeholder="Enter your message content..."
          value={formData.content}
          onChange={(e: React.ChangeEvent<HTMLTextAreaElement>) =>
            onFormDataChange({ ...formData, content: e.target.value })
          }
          rows={4}
          className={errors.content ? "border-red-500" : ""}
        />
        {errors.content && (
          <p className="text-xs text-red-500">{errors.content}</p>
        )}
        <p className="text-xs text-muted-foreground">
          {formData.content.length}/ 10 000 characters
        </p>
      </div>

      <div className="flex gap-2">
        <Button onClick={onSubmit} className="flex-1">
          {mode === "edit" ? "Update" : "Create"}
        </Button>
        <Button variant="outline" onClick={onCancel} className="flex-1">
          Cancel
        </Button>
      </div>
    </div>
  );
};
